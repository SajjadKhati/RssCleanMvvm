using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;

namespace Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface
{
    public interface IDraftCacheWriter
    {
        void AddOrUpdateToDraftCache<TKey, TItem>(IDraftCache<TKey, TItem> draftCache, TKey key, TItem item, DraftState draftState);
    }
}
