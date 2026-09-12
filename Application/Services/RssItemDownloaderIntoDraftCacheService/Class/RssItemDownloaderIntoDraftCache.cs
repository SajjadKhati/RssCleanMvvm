using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.RssItemDtoFactory.Interface;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.Services.RssItemDownloaderIntoDraftCacheService.DataModels;
using Application.Services.RssItemDownloaderIntoDraftCacheService.Interface;
using Application.Services.RssItemsDownLoaderService.Interface;
using Application.Services.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemDownloaderIntoDraftCacheService.Class
{
    public class RssItemDownloaderIntoDraftCache : IRssItemDownloaderIntoDraftCache
    {

        private IRssItemsDownLoader rssItemsDownLoader;

        private IRssItemDtoFactory rssItemDtoFactory;

        private IRssItemDraftCacheManager rssItemDraftCacheManager;

        private IImageSettingsReader imageSettingsReader;

        private readonly string draftImageFolderPath;


        //////////////////////////////////////////////////////////////////////////////


        public RssItemDownloaderIntoDraftCache(IRssItemsDownLoader rssItemsDownLoader, IRssItemDtoFactory rssItemDtoFactory, 
            IRssItemDraftCacheManager rssItemDraftCacheManager, IImageSettingsReader imageSettingsReader, string draftImageFolderPath)
        {
            this.rssItemsDownLoader = rssItemsDownLoader;
            this.rssItemDtoFactory = rssItemDtoFactory;
            this.rssItemDraftCacheManager = rssItemDraftCacheManager;
            this.imageSettingsReader = imageSettingsReader;
            this.draftImageFolderPath = draftImageFolderPath;
        }


        //////////////////////////////////////////////////////////////////////////////


        public async Task<RssItemsWithDtoReport> DownloadAndStoreInDraftCacheAsync(RssFeedConfig rssFeedConfig)
        {
            RssItemsReport rssItemsBuildingResult = await this.rssItemsDownLoader.DownLoadRssItemsAsync(rssFeedConfig);
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
                ImageSettings<ImageInfo> imageSettings = this.imageSettingsReader.GetOrDefault(rssItem.Id, rssItem.Argb8BitImage);
                ImageSaveOptions imageSaveOptions = new ImageSaveOptions(imageSettings, this.draftImageFolderPath);
                RssItemDto rssItemDto = this.rssItemDtoFactory.CreateRssItemDto(rssItem, imageSaveOptions);
                this.rssItemDraftCacheManager.MarkAsAddedInDraftCache(rssItem.Id, rssItem, rssItemDto);
                rssItemDtos.Add(rssItemDto);
            }
            return rssItemDtos;
        }

    }
}
