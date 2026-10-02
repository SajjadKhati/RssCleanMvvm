using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.UseCases.Entry.DownLoadRssItems.DataModels;
using Application.UseCases.Entry.DownLoadRssItems.Interface;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface;

namespace Application.UseCases.Entry.DownLoadRssItems.Class
{
    public class DownLoadRssItemsUseCase : IDownLoadRssItemsUseCase
    {

        private IRssItemDownloaderIntoDraftCacheOrchestrator rssItemDownloaderIntoDraftCacheOrchestrator;

        private readonly ICache<Uri, RssFeedConfig> rssFeedConfigCache;


        ///////////////////////////////////////////////////////////////////////////


        public DownLoadRssItemsUseCase(IRssItemDownloaderIntoDraftCacheOrchestrator rssItemDownloaderIntoDraftCacheOrchestrator, 
            ICache<Uri, RssFeedConfig> rssFeedConfigCache)
        {
            this.rssItemDownloaderIntoDraftCacheOrchestrator = rssItemDownloaderIntoDraftCacheOrchestrator;
            this.rssFeedConfigCache = rssFeedConfigCache;
        }


        ///////////////////////////////////////////////////////////////////////////


        public async Task<RssItemDtosResult> ExecuteAsync(Uri rssFeedId)
        {
            return await Task.Run(() => this.Execute(rssFeedId));
        }


        ///////////////////////////////////////////////////////////////////////////


        private async Task<RssItemDtosResult> Execute(Uri rssFeedId)
        {
            if (this.rssFeedConfigCache.ContainsKey(rssFeedId) == false)
                throw new Exception($"تنظیماتِ {nameof(RssFeedConfig)} برای این شناسه ی RssFeed ، پیدا نشد .");

            RssFeedConfig rssFeedConfig = this.rssFeedConfigCache.Get(rssFeedId);
            RssItemsWithDtoReport rssItemsWithDtoBuildingResult = 
                await this.rssItemDownloaderIntoDraftCacheOrchestrator.DownloadAndStoreInDraftCacheAsync(rssFeedConfig);
            return new RssItemDtosResult(rssItemsWithDtoBuildingResult.CreatedRssItemDtos, rssItemsWithDtoBuildingResult.CreationErrors);
        }

    }
}
