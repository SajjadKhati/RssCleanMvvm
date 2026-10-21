using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.Services.Mapper.RssItemDtosResultMapperService.Class;
using Application.UseCases.Entry.DownLoadRssItems.DataModels;
using Application.UseCases.Entry.DownLoadRssItems.Interface;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface;

namespace Application.UseCases.Entry.DownLoadRssItems.Class
{
    public class DownLoadRssItemsUseCase : IDownLoadRssItemsUseCase
    {

        private IRssItemDownloaderIntoDraftCacheOrchestrator rssItemDownloaderIntoDraftCacheOrchestrator;

        private RssItemDtosResultMapper rssItemDtosResultMapper;

        private readonly ICache<Uri, RssFeedConfig> rssFeedConfigCache;


        ///////////////////////////////////////////////////////////////////////////


        public DownLoadRssItemsUseCase(IRssItemDownloaderIntoDraftCacheOrchestrator rssItemDownloaderIntoDraftCacheOrchestrator,
            RssItemDtosResultMapper rssItemDtosResultMapper, ICache<Uri, RssFeedConfig> rssFeedConfigCache)
        {
            this.rssItemDownloaderIntoDraftCacheOrchestrator = rssItemDownloaderIntoDraftCacheOrchestrator;
            this.rssItemDtosResultMapper = rssItemDtosResultMapper;
            this.rssFeedConfigCache = rssFeedConfigCache;
        }


        ///////////////////////////////////////////////////////////////////////////


        public async Task<RssItemDtosResult> ExecuteAsync(Uri rssFeedId, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => this.Execute(rssFeedId, cancellationToken), cancellationToken);
        }


        ///////////////////////////////////////////////////////////////////////////


        private async Task<RssItemDtosResult> Execute(Uri rssFeedId, CancellationToken cancellationToken = default)
        {
            if (this.rssFeedConfigCache.TryGetValue(rssFeedId, out RssFeedConfig rssFeedConfig) == false || rssFeedConfig == null)
                throw new Exception($"تنظیماتِ {nameof(RssFeedConfig)} برای این شناسه ی RssFeed ، پیدا نشد .");

            RssItemWithDtosResult rssItemWithDtosResult = 
                await this.rssItemDownloaderIntoDraftCacheOrchestrator.DownloadAndStoreInDraftCacheAsync(rssFeedConfig, cancellationToken);
            return this.rssItemDtosResultMapper.ToRssItemDtosResult(rssItemWithDtosResult);
        }

    }
}
