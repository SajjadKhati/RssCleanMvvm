using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.Services.RssItemCreationServices.RssItemBuilderService.Interface;
using Application.Services.RssItemCreationServices.RssItemDataExtractorService.Interface;
using Domain.DomainEntities.RssEntity.Class;
using Domain.DomainService.RssItemDomainService.Interface;

namespace Application.Services.RssItemCreationServices.RssItemBuilderService.Class
{
    /// <summary>
    /// هماهنگ سازی یا orchestr یا همون استفاده از سرویس های دامنه برای اعتبارسنجی ، وظیفه ی این کلاس هست . 
    /// یعنی این کار ، وظیفه ی کلاس RssItemDataExtractor نیست .
    /// </summary>
    public class RssItemBuilder : IRssItemBuilder
    {

        private IRssItemDomainService rssItemDomainService;

        private IRssItemDataExtractor rssItemDataExtractor;


        //////////////////////////////////////////////////////////


        public RssItemBuilder(IRssItemDomainService rssItemDomainService, IRssItemDataExtractor rssItemDataExtractor)
        {
            this.rssItemDomainService = rssItemDomainService;
            this.rssItemDataExtractor = rssItemDataExtractor;
        }


        //////////////////////////////////////////////////////////


        public async Task<RssItem> CreateRssItemAsync(IDictionary<string, string> rssItemDictionary, 
            RssItemWithFeedIdFieldKeyConfig rssItemWithFeedIdFieldKeyConfig)
        {
            RssItemFieldKeyConfig rssItemFieldKeyConfig = rssItemWithFeedIdFieldKeyConfig.RssItemFieldKeyConfig;
            Uri rssFeedId = rssItemWithFeedIdFieldKeyConfig.RssFeedId;

            Uri id = this.rssItemDataExtractor.GetId(rssItemDictionary[rssItemFieldKeyConfig.IdKey]);
            if (this.rssItemDomainService.IsRssItemValid(id) == false)
                throw new InvalidDataException("این شیِ RssItem ، قبلا ساخته شد و شناسه اش موجود است .");

            Uri link = this.rssItemDataExtractor.GetLink(rssItemDictionary[rssItemFieldKeyConfig.LinkKey]);
            string category = rssItemDictionary[rssItemFieldKeyConfig.CategoryKey];
            string title = rssItemDictionary[rssItemFieldKeyConfig.TitleKey];
            string description = rssItemDictionary[rssItemFieldKeyConfig.DescriptionKey];
            DateTime publishDate = this.rssItemDataExtractor.GetPublishDate(rssItemDictionary[rssItemFieldKeyConfig.PublishDateKey]);
            byte[,,] argb8BitImage = await this.rssItemDataExtractor.GetArgb8BitImageAsync(rssItemDictionary[rssItemFieldKeyConfig.Argb8BitImageKey]);

            RssItem rssItem = new RssItem(id, rssFeedId, link, argb8BitImage, category, title, description, publishDate);
            return rssItem;
        }

    }
}
