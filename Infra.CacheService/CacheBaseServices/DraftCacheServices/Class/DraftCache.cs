using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;
using Infra.CacheService.CacheBaseServices.CacheServices.Class;

namespace Infra.CacheService.CacheBaseServices.DraftCacheServices.Class
{
    public class DraftCache<TKey, TItem> : Cache<TKey, DraftEntry<TItem>>, IDraftCache<TKey, TItem>
        where TKey : notnull
    {

        public DraftCache(int collectionCapacity = -1) : base(collectionCapacity)
        {
        }


        public DraftCache(IEnumerable<KeyValuePair<TKey, DraftEntry<TItem>>> collection) : base(collection)
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
