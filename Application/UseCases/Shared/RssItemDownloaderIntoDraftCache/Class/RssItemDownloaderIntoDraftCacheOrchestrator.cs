using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface;
using Application.UseCases.Shared.RssItemDtoFactory.Interface;
using Application.UseCases.Shared.RssItemsDownLoader.Interface;
using Application.UseCases.Shared.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Class
{
    public class RssItemDownloaderIntoDraftCacheOrchestrator : IRssItemDownloaderIntoDraftCacheOrchestrator
    {

        private IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator;

        private IRssItemDtoFactoryOrchestrator rssItemDtoFactoryOrchestrator;

        private IRssItemDraftCacheManager rssItemDraftCacheManager;


        //////////////////////////////////////////////////////////////////////////////


        public RssItemDownloaderIntoDraftCacheOrchestrator(IRssItemsDownLoaderOrchestrator rssItemsDownLoaderOrchestrator,
            IRssItemDtoFactoryOrchestrator rssItemDtoFactoryOrchestrator, IRssItemDraftCacheManager rssItemDraftCacheManager)
        {
            this.rssItemsDownLoaderOrchestrator = rssItemsDownLoaderOrchestrator;
            this.rssItemDtoFactoryOrchestrator = rssItemDtoFactoryOrchestrator;
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
        }


        //////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemsWithDtoReport> DownloadAndStoreInDraftCacheAsync(RssFeedConfig rssFeedConfig)
        {
            RssItemsReport rssItemsBuildingResult = await this.rssItemsDownLoaderOrchestrator.DownLoadRssItemsAsync(rssFeedConfig);
            if (rssItemsBuildingResult.CreatedRssItems == null || rssItemsBuildingResult.CreatedRssItems.Count < 1)
                return new RssItemsWithDtoReport(null, null, rssItemsBuildingResult.CreationErrors);
            IList<RssItemDto> rssItemDtos = this.CreateDtosAndStoreInDraftCache(rssItemsBuildingResult.CreatedRssItems);
            return new RssItemsWithDtoReport(rssItemsBuildingResult.CreatedRssItems, rssItemDtos, rssItemsBuildingResult.CreationErrors);
        }


        //////////////////////////////////////////////////////////////////////////////


        private IList<RssItemDto> CreateDtosAndStoreInDraftCache(IList<RssItem> rssItems)
        {
            List<RssItemDto> rssItemDtos = new List<RssItemDto>();
            foreach (RssItem rssItem in rssItems)
            {
                RssItemDto rssItemDto = this.rssItemDtoFactoryOrchestrator.CreateRssItemDto(rssItem);
                this.rssItemDraftCacheManager.MarkAsAddedInDraftCache(rssItem.Id, rssItem, rssItemDto);
                rssItemDtos.Add(rssItemDto);
            }
            return rssItemDtos;
        }

    }
}
