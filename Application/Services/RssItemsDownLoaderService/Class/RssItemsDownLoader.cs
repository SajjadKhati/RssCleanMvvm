using Application.InfrastructureContracts.HttpProject.HttpRequest.Factory.Interfaces;
using Application.InfrastructureContracts.HttpProject.HttpRequest.Interfaces;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.XmlParser.Interfaces;
using Application.Services.RssItemCreationServices.RssItemBuilderService.Interface;
using Application.Services.RssItemsDownLoaderService.Interface;
using Application.Services.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemsDownLoaderService.Class
{
    public class RssItemsDownLoader : IRssItemsDownLoader
    {

        private IHttpRequestFactory httpRequestFactory;

        private IXmlParser xmlParser;

        private IRssItemBuilder rssItemBuilder;


        ////////////////////////////////////////////////////////////////////////////


        public RssItemsDownLoader(IHttpRequestFactory httpRequestFactory, IXmlParser xmlParser, IRssItemBuilder rssItemBuilder)
        {
            this.httpRequestFactory = httpRequestFactory;
            this.xmlParser = xmlParser;
            this.rssItemBuilder = rssItemBuilder;
        }


        ////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemsReport> DownLoadRssItemsAsync(RssFeedConfig rssFeedConfig)
        {
            RssFeedXmlParsingConfig rssFeedXmlParsingConfig = rssFeedConfig.RssFeedXmlParsingConfig;
            RssItemWithFeedIdFieldKeyConfig rssItemFieldKeyConfig = rssFeedConfig.RssItemWithFeedIdFieldKeyConfig;
            string rssFeedXmlData = await this.DownloadRssFeedXmlData(rssFeedXmlParsingConfig.RssFeedId);
            XmlParsingData xmlParsingData = new XmlParsingData(rssFeedXmlData, rssFeedXmlParsingConfig);
            IList<IDictionary<string, string>> rawRssItems = this.xmlParser.GetXmlData(xmlParsingData);
            RssItemsReport rssItemsBuildingResult = await this.BuildRssItems(rawRssItems, rssItemFieldKeyConfig);
            return rssItemsBuildingResult;
        }


        ////////////////////////////////////////////////////////////////////////////


        private async Task<string> DownloadRssFeedXmlData(Uri rssFeedId)
        {
            using IHttpRequest httpRequest = this.httpRequestFactory.Create();
            return await httpRequest.GetStringAsync(rssFeedId);
        }


        private async Task<RssItemsReport> BuildRssItems(IList<IDictionary<string, string>> rawRssItems, 
            RssItemWithFeedIdFieldKeyConfig rssItemFieldKeyConfig)
        {
            List<RssItem> createdRssItems = new List<RssItem>();
            List<Exception> creationErrors = new List<Exception>();
            foreach (IDictionary<string, string> rawRssItem in rawRssItems)
            {
                try
                {
                    RssItem rssItem = await this.rssItemBuilder.CreateRssItemAsync(rawRssItem, rssItemFieldKeyConfig);
                    createdRssItems.Add(rssItem);
                }
                catch (Exception exception)
                {
                    creationErrors.Add(exception);
                }
            }
            return new RssItemsReport(createdRssItems, creationErrors);
        }

    }
}
