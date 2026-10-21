using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Interface;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface;
using Application.UseCases.Shared.RssItemsDownLoader.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Class
{
    public class RssItemDownloaderIntoDraftCacheOrchestrator : IRssItemDownloaderIntoDraftCacheOrchestrator
    {

        private IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator;

        private IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator;

        private IRssItemDraftCacheWriter rssItemDraftCacheWriter;


        //////////////////////////////////////////////////////////////////////////////


        public RssItemDownloaderIntoDraftCacheOrchestrator(IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator,
            IDraftRssItemDtoFactoryOrchestrator draftRssItemDtoFactoryOrchestrator, IRssItemDraftCacheWriter rssItemDraftCacheWriter)
        {
            this.rssItemsDownLoaderOrchestrator = rssItemsDownLoaderOrchestrator;
            this.draftRssItemDtoFactoryOrchestrator = draftRssItemDtoFactoryOrchestrator;
            this.rssItemDraftCacheWriter = rssItemDraftCacheWriter;
        }


        //////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemWithDtosResult> DownloadAndStoreInDraftCacheAsync(RssFeedConfig rssFeedConfig, 
            CancellationToken cancellationToken = default)
        {
            List<RssItemWithDto> createdRssItemWithDtos = new List<RssItemWithDto>();
            List<Exception> creationErrors = new List<Exception>();
            bool wasUserCancelled = false;

            /// ⚠️ چرا دو try-catch تودرتو؟
            ///
            /// چون دو نوع خطای متفاوت داریم و هر کدام معنی متفاوتی دارند:
            ///
            ///   • try/catch داخلی (داخل حلقه):
            ///     خطاهای «per-item» — یعنی خطایی که فقط برای یک آیتم رخ داده.
            ///     اگه این catch نبود، خطای یک آیتم، کل حلقه رو متوقف می‌کرد
            ///     و بقیه‌ی آیتم‌ها هرگز پردازش نمی‌شدن.
            ///     هدف: resilience — یک آیتم خراب، بقیه رو خراب نکنه.
            ///
            ///   • try/catch بیرونی (دور کل حلقه):
            ///     خطاهای «batch-wide» — یعنی خطاهایی که کل عملیات رو تحت تأثیر قرار می‌دهند:
            ///       - AggregateException: خطاهای ساخت RssItem (از BuildRssItems)
            ///         که بعد از yield همه‌ی آیتم‌های موفق پرتاب می‌شه.
            ///       - OperationCanceledException: لغو کاربر.
            ///     این‌ها با catch داخلی گرفته نمی‌شن، چون از iterator میان، نه از بدنه‌ی حلقه.
            try
            {
                await foreach (RssItem rssItem in this.rssItemsDownLoaderOrchestrator.DownLoadRssItemsAsync(rssFeedConfig, cancellationToken))
                {
                    if (rssItem == null)
                        continue;
                    try
                    {
                        RssItemWithDto rssItemWithDto = this.CreateRssItemWithDtoAndStoreInDraftCache(rssItem);
                        if (rssItemWithDto == null)
                            continue;
                        createdRssItemWithDtos.Add(rssItemWithDto);
                    }
                    /// ⚠️ چرا catch (Exception) و نه چیز دیگه؟
                    ///
                    /// ساخت DTO ممکنه به هر دلیلی شکست بخوره:
                    ///   - خطای ذخیره‌ی فایل تصویر (دیسک پر، دسترسی نداریم)
                    ///   - خطای تبدیل تاریخ به شمسی
                    ///   - خطای ساخت مسیر فایل
                    /// اینا همه از نوع Exception هستن. برای resilience، همه رو می‌گیریم
                    /// و به creationErrors اضافه می‌کنیم، تا بقیه‌ی آیتم‌ها ادامه پیدا کنن.
                    ///
                    /// why when filter:
                    /// CreateDtoAndStoreInDraftCache الان sync هست و OCE پرتاب نمی‌کنه،
                    /// ولی اگه یه روز async بشه و CancellationToken بگیره، OCE ممکنه
                    /// پرتاب بشه. اون‌وقت باید propagate بشه (نه به‌عنوان خطای آیتم ثبت بشه)،
                    /// تا لایه‌ی بالاتر بفهمه «کاربر لغو کرد»، نه «خطای ساخت DTO».
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        creationErrors.Add(exception);
                    }
                }
            }
            /// ⚠️ چرا AggregateException رو جدا گرفتیم؟
            ///
            /// BuildRssItems (در RssItemsDownLoaderOrchestrator) خطاهای ساخت
            /// RssItemها رو جمع می‌کنه و در پایان، بعد از yield همه‌ی موفق‌ها،
            /// یه AggregateException پرتاب می‌کنه.
            ///
            /// چرا اینجا گرفته می‌شه و نه در catch داخلی؟
            /// چون AggregateException از iterator (بیرون از حلقه) پرتاب می‌شه،
            /// نه از بدنه‌ی حلقه. پس catch داخلی نمی‌تونه بگیره.
            ///
            /// چرا InnerExceptions گرفته می‌شه (نه خود AggregateException)؟
            /// چون InnerExceptions شامل خطاهای تک‌تکِ هر آیتمه، نه یه خطای تجمیعی.
            /// کاربر می‌خواد «متن خطای اول» رو ببینه، نه «3 از 10 آیتم خراب شد».
            catch (AggregateException aggregateException)
            {
                creationErrors.AddRange(aggregateException.InnerExceptions);
            }
            /// ⚠️ چرا OperationCanceledException رو گرفتیم و به errors اضافه نکردیم؟
            ///
            /// چون لغو، «خطا» نیست — «دستور کنترلی»ـه. کاربر عمداً لغو کرده،
            /// این یه حالت مورد انتظاره، نه یه شکست.
            ///
            /// اگه به creationErrors اضافه می‌کردیم، VM فکر می‌کرد
            /// «خطا رخ داد» و پیام خطا نشون می‌داد. ولی کاربر باید پیام
            /// «عملیات لغو شد» ببینه.
            ///
            /// پس با WasUserCancelled = true، به VM می‌گیم این لغو بوده، نه خطا.
            catch (OperationCanceledException)
            {
                wasUserCancelled = true;
            }
            /// ⚠️ چرا catch (Exception) بیرونی نداریم؟
            ///
            /// چون خطاهای قبل از حلقه (DownloadRssFeedXmlData، GetXmlData) نباید
            /// به‌عنوان «نتیجه‌ی ناقص» برگردونده بشن.
            ///
            /// اگه این خطاها رو catch می‌کردیم و RssItemsWithDtoResult با لیست خالی
            /// برمی‌گردوندیم، VM فکر می‌کرد «هیچ خبر جدیدی نیست» — در حالی که
            /// واقعیت «خطای شبکه» بود. این پیام، گمراه‌کننده‌ست.
            ///
            /// پس این خطاها عمداً propagate می‌شن تا VM (با catch (Exception) خودش)
            /// پیام مناسب («خطای شبکه») رو نشون بده.
            return new RssItemWithDtosResult(createdRssItemWithDtos, creationErrors, wasUserCancelled);
        }


        //////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// ساخت DTOها برای آیتم‌های دانلودشده و ذخیره در کش موقت.
        /// 
        /// ⚠️ چرا قفل ندارد؟
        /// 
        /// دلیل اول: عملیات کند (CreateRssItemDto) قبل از دسترسی به داده‌ی مشترک 
        /// انجام می‌شود، نه بین دو دسترسی. قفل فقط وقتی لازم است که یک عملیات کند، 
        /// وسط دو دسترسی به همان داده باشد.
        /// 
        /// دلیل دوم: هر فراخوانی MarkAsAddedInDraftCache خودش اتمی است (write lock 
        /// داخلی‌اش). پس نیازی به قفل بیرونی ندارد.
        /// 
        /// دلیل سوم: هیچ قانونی بین آیتم‌های حلقه نیست — هر RssItem با Id خودش 
        /// مستقل است. پس حتی اگر بین iterateها، نخ دیگری چیزی را تغییر دهد، 
        /// هیچ چیزی در این حلقه خراب نمی‌شود.
        /// </summary>
        /// <param name="rssItem"></param>
        /// <returns></returns>
        private RssItemWithDto CreateRssItemWithDtoAndStoreInDraftCache(RssItem rssItem)
        {
            RssItemDto rssItemDto = this.draftRssItemDtoFactoryOrchestrator.CreateRssItemDtoWithSaveImage(rssItem);
            RssItemWithDto rssItemWithDto = new RssItemWithDto(rssItem, rssItemDto);

            /// اکر مقدار originalDraftEntry را nullارسال کنیم ، یعنی «انتظار داریم اگر rssItemWithDto در cache نبود.» در این صورت ، اضافه کند .
            /// هر چند در متد CreateRssItemAsync در کلاس RssItemBuilder ، قبلا چک کرده بودیم که در کش موقت و دیتابیس وجود نداشته باشد ، -
            /// اما ممکن هست نخ دیگری در این فاصله ، این آیتم را زودتر دانلود کنه و در کش موقت ذخیره کنه . 
            /// هر چند ، کامند در VM جلوی این اجرا را میگیره ولی کار از محکم کاری عیب نمیکنه ...
            /// پس متد TryMarkAsAddedIfDraftEntryUnchanged ، اگر آیتم از قبل موجود باشه و فرضا اگر نخ دیگری در این فاصله اضافه کرده باشد، false برمیگرداند و به کش موقت ، اضافه نمیکنه .
            bool isAddedToDraftCache = this.rssItemDraftCacheWriter.TryMarkAsAddedIfDraftEntryUnchanged(
                rssItem.Id, rssItemWithDto, originalDraftEntry: null);
            return (isAddedToDraftCache == true)? rssItemWithDto : null;
        }

    }
}
