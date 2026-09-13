using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;
using Application.InfrastructureContracts.SharedProject.FileManager.Interface;
using Application.InfrastructureContracts.SharedProject.FolderManager.Interface;
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

        private IFileManager fileManager;

        private IFolderManager folderManager;

        private string tempImagesFolderPath;

        #endregion


        ////////////////////////////////////////////////////////////////


        #region متد سازنده

        public RssItemDraftCacheManager(IDraftCache<Uri, RssItem> rssItemDraftCache, IDraftCache<Uri, RssItemDto> rssItemDtoDraftCache, 
            IDraftCacheWriter draftCacheWriter, IFileManager fileManager, IFolderManager folderManager, string tempImagesFolderPath)
        {
            this.rssItemDraftCache = rssItemDraftCache;
            this.rssItemDtoDraftCache = rssItemDtoDraftCache;
            this.draftCacheWriter = draftCacheWriter;
            this.fileManager = fileManager;
            this.folderManager = folderManager;
            this.tempImagesFolderPath = tempImagesFolderPath;
        }

        #endregion


        ////////////////////////////////////////////////////////////////


        public void MarkAsAddedInDraftCache(Uri key, RssItem rssItem, RssItemDto rssItemDto)
        {
            this.AddOrUpdateToDraftCaches(key, rssItem, rssItemDto, DraftState.Added);
        }

        public void MarkAsModifiedInDraftCache(Uri key, RssItem rssItem, RssItemDto rssItemDto)
        {
            this.AddOrUpdateToDraftCaches(key, rssItem, rssItemDto, DraftState.Modified);
        }

        public void MarkAsDeletedInDraftCache(Uri key, RssItem rssItem, RssItemDto rssItemDto)
        {
            this.AddOrUpdateToDraftCaches(key, rssItem, rssItemDto, DraftState.Deleted);
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
            this.folderManager.DeleteFolderWithContents(this.tempImagesFolderPath);
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


        public IReadOnlyList<RssItemDto> GetRssItemDtosByFeedId(Uri rssFeedId)
        {
            IList<Uri> draftCacheKeysByFeedId = this.GetDraftCacheKeysByFeedId(rssFeedId);
            if (draftCacheKeysByFeedId == null || draftCacheKeysByFeedId.Count < 1)
                return null;
            return this.GetDraftRssItemDtosByKeys(draftCacheKeysByFeedId);
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


        private IList<Uri> GetDraftCacheKeysByFeedId(Uri rssFeedId)
        {
            return this.rssItemDraftCache.GetAllKeyValuesSnapshot()?.
                Where(keyValuePairs => keyValuePairs.Value.Item.RssFeedId == rssFeedId)?.
                Select(keyValuePairs => keyValuePairs.Key)?.
                ToList();
        }


        private IReadOnlyList<RssItemDto> GetDraftRssItemDtosByKeys(IList<Uri> draftCacheKeys)
        {
            return this.rssItemDtoDraftCache.GetAllKeyValuesSnapshot()?.
                Where(keyValuePairs => draftCacheKeys.Contains(keyValuePairs.Key))?.
                Select(keyValuePairs => keyValuePairs.Value.Item)?.
                ToList();
        }

    }
}
