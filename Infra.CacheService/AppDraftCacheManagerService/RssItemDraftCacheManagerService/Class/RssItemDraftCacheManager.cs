using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.SharedProject.FileManager.Interface;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;
using Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Infra.CacheService.AppDraftCacheManagerService.RssItemDraftCacheManagerService.Class
{
    public class RssItemDraftCacheManager : IRssItemDraftCacheManager
    {

        #region فیلدها

        private IDraftCache<Uri, RssItem> rssItemDraftCache;

        private IDraftCache<Uri, RssItemDto> rssItemDtoDraftCache;

        private IDraftCacheWriter draftCacheWriter;

        private ImageSaveOptions imageSaveOptions;

        private IImageFileSaverCoordinator imageFileSaverCoordinator;

        private IRssItemDtoMapper rssItemDtoMapper;

        private IFileManager fileManager;

        #endregion


        ////////////////////////////////////////////////////////////////


        #region متد سازنده

        public RssItemDraftCacheManager(IDraftCache<Uri, RssItem> rssItemDraftCache,
            IDraftCache<Uri, RssItemDto> rssItemDtoDraftCache, IDraftCacheWriter draftCacheWriter, ImageSaveOptions imageSaveOptions, 
            IImageFileSaverCoordinator imageFileSaverCoordinator, IRssItemDtoMapper rssItemDtoMapper, 
            IFileManager fileManager)
        {
            this.rssItemDraftCache = rssItemDraftCache;
            this.rssItemDtoDraftCache = rssItemDtoDraftCache;
            this.draftCacheWriter = draftCacheWriter;
            this.imageSaveOptions = imageSaveOptions;
            this.imageFileSaverCoordinator = imageFileSaverCoordinator;
            this.rssItemDtoMapper = rssItemDtoMapper;
            this.fileManager = fileManager;
        }

        #endregion


        ////////////////////////////////////////////////////////////////


        public void MarkAsAddedInDraftCache(Uri key, RssItem item)
        {
            RssItemDto rssItemDto = this.CreateRssItemDto(item);
            this.AddOrUpdateToDraftCaches(key, item, rssItemDto, DraftState.Added);
        }


        public void MarkAsModifiedInDraftCache(Uri key, RssItem item)
        {
            RssItemDto rssItemDto = this.CreateRssItemDto(item);
            this.AddOrUpdateToDraftCaches(key, item, rssItemDto, DraftState.Modified);
        }


        public void MarkAsDeletedInDraftCache(Uri key, RssItem item)
        {
            RssItemDto rssItemDto = this.CreateRssItemDto(item);
            this.AddOrUpdateToDraftCaches(key, item, rssItemDto, DraftState.Deleted);
        }


        public void Remove(Uri key)
        {
            if (this.rssItemDtoDraftCache.ContainsKey(key) == false || this.rssItemDraftCache.ContainsKey(key) == false)
                throw new KeyNotFoundException("آیتمی با این کلید در سرویس کشِ موقتِ RssItemDto یا در کشِ موقتِ RssItem وجود ندارد .");
            string draftImageFullFilePath = this.rssItemDtoDraftCache.GetItem(key).ImageFullFilePath;

            this.DeleteDraftImageFile(draftImageFullFilePath);

            this.rssItemDraftCache.Remove(key);
            this.rssItemDtoDraftCache.Remove(key);
        }


        public void Clear()
        {
            foreach (var rssItemDtoKeyValuePair in this.rssItemDtoDraftCache.GetAllKeyValuesSnapshot())
            {
                this.DeleteDraftImageFile(rssItemDtoKeyValuePair.Value.Item.ImageFullFilePath);
            }

            this.rssItemDraftCache.Clear();
            this.rssItemDtoDraftCache.Clear();
        }


        public bool ContainKey(Uri key)
        {
            return this.rssItemDraftCache.ContainsKey(key);
        }


        public DraftEntry<RssItem> GetRssItemDraftEntry(Uri key)
        {
            return this.rssItemDraftCache.Get(key);
        }


        public IReadOnlyList<DraftEntry<RssItem>> GetAllRssItemDraftEntries()
        {
            return this.rssItemDraftCache.GetAll();
        }


        public RssItemDto GetRssItemDto(Uri key)
        {
            return this.rssItemDtoDraftCache.GetItem(key);
        }


        public IReadOnlyList<RssItemDto> GetAllRssItemDto()
        {
            return this.rssItemDtoDraftCache.GetAllItems();
        }


        public void Dispose()
        {
            this.Dispose(true);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing == false)
                return;
            this.Clear();
        }


        ////////////////////////////////////////////////////////////////


        private RssItemDto CreateRssItemDto(RssItem rssItem)
        {
            DestinationPath draftImageDestinationPath = this.SaveImageToFile(rssItem.Argb8BitImage, this.imageSaveOptions.FolderPath,
                rssItem.Id.AbsoluteUri, this.imageSaveOptions.ImageInfoSettings);
            return this.rssItemDtoMapper.ToRssItemDto(rssItem, draftImageDestinationPath.FullFilePath);
        }

        private DestinationPath SaveImageToFile(byte[,,] image, string imageFolderPath, string imageFileName, ImageSettings<ImageInfo> imageInfoSettings)
        {
            var imageFilePath = new FilePath(imageFolderPath, imageFileName);
            DestinationPath imageDestinationPath = this.imageFileSaverCoordinator.Save8BitImageToFile(image, imageInfoSettings, imageFilePath);
            return imageDestinationPath;
        }


        private void AddOrUpdateToDraftCaches(Uri key, RssItem rssItem, RssItemDto rssItemDto, DraftState draftState)
        {
            this.draftCacheWriter.AddOrUpdateToDraftCache(this.rssItemDraftCache, key, rssItem, draftState);
            this.draftCacheWriter.AddOrUpdateToDraftCache(this.rssItemDtoDraftCache, key, rssItemDto, draftState);
        }


        private void DeleteDraftImageFile(string draftImageFullFilePath)
        {
            if (string.IsNullOrEmpty(draftImageFullFilePath) == false)
            {
                this.fileManager.DeleteFile(draftImageFullFilePath);
            }
        }

    }
}
