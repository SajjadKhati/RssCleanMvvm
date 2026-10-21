using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Application.Services.RssItemCreationServices.RssItemBuilderService.Interface;
using Application.UseCases.Shared.RssItemsDownLoader.Interface;
using Domain.DomainEntities.RssEntity.Class;
using System.Runtime.CompilerServices;

namespace Application.UseCases.Shared.RssItemsDownLoader.Class
{
    public class RssItemsDownLoaderOrchestrator : IRssItemsDownLoaderOrchestrator
    {

        private IHttpRequestFactory httpRequestFactory;

        private IXmlParser xmlParser;

        private IRssItemBuilder rssItemBuilder;


        ////////////////////////////////////////////////////////////////////////////


        public RssItemsDownLoaderOrchestrator(IHttpRequestFactory httpRequestFactory, IXmlParser xmlParser, IRssItemBuilder rssItemBuilder)
        {
            this.httpRequestFactory = httpRequestFactory;
            this.xmlParser = xmlParser;
            this.rssItemBuilder = rssItemBuilder;
        }


        ////////////////////////////////////////////////////////////////////////////


        public async IAsyncEnumerable<RssItem> DownLoadRssItemsAsync(RssFeedConfig rssFeedConfig,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            RssFeedXmlParsingConfig rssFeedXmlParsingConfig = rssFeedConfig.RssFeedXmlParsingConfig;
            RssItemWithFeedIdFieldKeyConfig rssItemFieldKeyConfig = rssFeedConfig.RssItemWithFeedIdFieldKeyConfig;
            string rssFeedXmlData = await this.DownloadRssFeedXmlData(rssFeedXmlParsingConfig.RssFeedId, cancellationToken);
            if (string.IsNullOrWhiteSpace(rssFeedXmlData) == true)
                yield break;
            XmlParsingData xmlParsingData = new XmlParsingData(rssFeedXmlData, rssFeedXmlParsingConfig);
            IList<IDictionary<string, string>> rawRssItems = this.xmlParser.GetXmlData(xmlParsingData);
            if (rawRssItems == null)
                yield break;

            await foreach (RssItem rssItem in this.BuildRssItems(rawRssItems, rssItemFieldKeyConfig, cancellationToken))
            {
                yield return rssItem;
            }
        }


        ////////////////////////////////////////////////////////////////////////////


        private async Task<string> DownloadRssFeedXmlData(Uri rssFeedId, CancellationToken cancellationToken = default)
        {
            using IHttpRequest httpRequest = this.httpRequestFactory.Create();
            return await httpRequest.GetStringAsync(rssFeedId, cancellationToken);
        }


        /// <summary>
        /// ساخت تدریجی RssItemها از داده‌ی خام XML.
        ///
        /// چرا yield؟
        ///   کاربر باید بتونه هر آیتم رو به‌محض آماده شدن دریافت کنه
        ///   (برای ذخیره‌ی تدریجی در DraftCache و لغو فوری).
        ///   اگه همه‌ی آیتم‌ها رو جمع می‌کردیم و آخر یه لیست برمی‌گردوندیم،
        ///   لغو تا آخر صبر می‌کرد و آیتم‌های ساخته‌شده از دست می‌رفتند.
        ///
        /// چرا خطاها فوری پرتاب نمی‌شن؟
        ///   یه آیتم خراب (مثلاً تاریخ نامعتبر) نباید بقیه‌ی آیتم‌ها رو متوقف کنه.
        ///   پس خطاها جمع می‌شن و در پایان (بعد از yield همه‌ی موفق‌ها)
        ///   به‌صورت AggregateException پرتاب می‌شن.
        ///
        /// چرا OperationCanceledException گرفته نمی‌شه؟
        ///   لغو کاربر، یه دستور کنترلیه، نه یه خطا. اگه گرفته بشه،
        ///   توی AggregateException پیچیده می‌شه و لایه‌ی بالا (VM)
        ///   نمی‌فهمه «کاربر لغو کرد» — به‌جاش «خطا» نشون می‌ده.
        ///   پس با when filter، عمداً propagate می‌شه.
        /// </summary>
        /// <param name="rawRssItems"></param>
        /// <param name="rssItemFieldKeyConfig"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        private async IAsyncEnumerable<RssItem> BuildRssItems(IList<IDictionary<string, string>> rawRssItems, 
            RssItemWithFeedIdFieldKeyConfig rssItemFieldKeyConfig, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            List<Exception> creationErrors = new List<Exception>();
            foreach (IDictionary<string, string> rawRssItem in rawRssItems)
            {
                /// لغو کاربر → فوری propagate (نه داخل AggregateException)
                cancellationToken.ThrowIfCancellationRequested();
                RssItem rssItem = null;
                try
                {
                    rssItem = await this.rssItemBuilder.CreateRssItemAsync(rawRssItem, rssItemFieldKeyConfig, cancellationToken);
                }
                /// به شرطی که نوع خطا ، از نوع OperationCanceledException نباشه ( که با لغو کاربر ، این خطا پرتاب میشه) ، خطاهای نوع Exception را میگیره .
                /// لغو کاربر نباید به‌عنوان خطای ساخت ثبت بشه.
                /// با when filter، OperationCanceledException از این catch رد می‌شه
                /// و به‌صورت خام به لایه‌ی بالا می‌ره (تا VM پیام «لغو شد» بده، نه «خطا»).
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    creationErrors.Add(exception);
                }
                if (rssItem == null)
                    continue;
                yield return rssItem;
            }
            /// اینجا همه‌ی آیتم‌های موفق yield شدن.
            /// حالا اگه خطایی بود، یه‌جا پرتاب می‌شه تا لایه‌ی بالا (UseCase) بتونه
            /// بین «همه موفق»، «بعضی خطا»، و «لغو» تفکیک کنه.
            if (creationErrors.Count > 0)
                throw new AggregateException($"{creationErrors.Count} مورد خبری از {rawRssItems.Count} مورد ، ساخته نشد.", creationErrors);
        }

    }
}
