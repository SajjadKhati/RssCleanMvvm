using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;

namespace Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces
{
    public interface IDraftCache<TKey, TItem> : ICache<TKey, DraftEntry<TItem>>
        where TKey : notnull
    {

        TItem GetItem(TKey key);

        IReadOnlyList<TItem> GetAllItems();

    }
}
