using Application.InfrastructureContracts.CacheProject.Cache.Interfaces;
using Application.InfrastructureContracts.CacheProject.DraftCache.DataModels;

namespace Application.InfrastructureContracts.CacheProject.DraftCache.Interfaces
{
    public interface IDraftCache<TKey, TItem> : ICache<TKey, DraftEntry<TItem>>
        where TKey : notnull
    {

        TItem GetItem(TKey key);

        IReadOnlyList<TItem> GetAllItems();

    }
}
