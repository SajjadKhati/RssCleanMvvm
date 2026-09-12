using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.Services.RssItemDownloaderIntoDraftCacheService.DataModels;
using Application.Services.RssItemDownloaderIntoDraftCacheService.Interface;
using Application.UseCases.DownLoadRssItemsUseCaseModule.DataModels;
using Application.UseCases.DownLoadRssItemsUseCaseModule.Interface;

namespace Application.UseCases.DownLoadRssItemsUseCaseModule.Class
{
    public class DownLoadRssItemsUseCase : IDownLoadRssItemsUseCase
    {

        private IRssItemDownloaderIntoDraftCache rssItemDownloaderIntoDraftCache;

        private readonly ICache<Uri, RssFeedConfig> rssFeedConfigCache;


        ///////////////////////////////////////////////////////////////////////////


        public DownLoadRssItemsUseCase(IRssItemDownloaderIntoDraftCache rssItemDownloaderIntoDraftCache, 
            ICache<Uri, RssFeedConfig> rssFeedConfigCache)
        {
            this.rssItemDownloaderIntoDraftCache = rssItemDownloaderIntoDraftCache;
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
                await this.rssItemDownloaderIntoDraftCache.DownloadAndStoreInDraftCacheAsync(rssFeedConfig);
            return new RssItemDtosResult(rssItemsWithDtoBuildingResult.CreatedRssItemDtos, rssItemsWithDtoBuildingResult.CreationErrors);
        }

    }
}
