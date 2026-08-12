using Application.InfrastructureContracts.CacheProject.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.DraftCache.Interfaces;
using Infra.CacheService.CacheServices.Class;

namespace Infra.CacheService.DraftCacheServices.Class
{
    public class DraftCache<TKey, TItem> : Cache<TKey, DraftEntry<TItem>>, IDraftCache<TKey, TItem>
        where TKey : notnull
    {

        public DraftCache(int collectionCapacity = -1) : base(collectionCapacity)
        {
        }


        ////////////////////////////////////////////////


        public TItem GetItem(TKey key)
        {
            return this.Get(key).Item;
        }


        public IReadOnlyList<TItem> GetAllItems()
        {
            return this.itemsDictionary.Values.Select(draftEntry => draftEntry.Item).ToList().AsReadOnly();
        }

    }
}
