using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;

namespace Infra.CacheService.AppDraftCacheManagerService.DraftCacheWriterService.Class
{
    public class DraftCacheWriter : IDraftCacheWriter
    {

        public void AddOrUpdateToDraftCache<TKey, TItem>(IDraftCache<TKey, TItem> draftCache, TKey key, TItem item, DraftState draftState)
        {
            DraftEntry<TItem> draftEntry = new DraftEntry<TItem>(item, draftState, key);
            draftCache.AddOrUpdate(key, draftEntry);
        }

    }
}
