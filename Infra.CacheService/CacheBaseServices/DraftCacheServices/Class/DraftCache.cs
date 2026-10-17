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


        public bool TryGetItem(TKey key, out TItem value)
        {
            if(this.TryGetValue(key, out DraftEntry<TItem> draftEntry) == true)
            {
                value = draftEntry.Item;
                return true;
            }
            value = default(TItem);
            return false;
        }


        public IReadOnlyList<TItem> GetAllItemsCopy()
        {
            /// همانند GetAllValuesCopy در کلاس Cache:
            /// Values ابتدا یک snapshot می‌گیرد، سپس Select و ToList و AsReadOnly
            /// روی همان snapshot کار می‌کنند. تغییرات همزمان نخ‌های دیگر روی itemsDictionary،
            /// به این snapshot آسیب نمی‌زند. پس این زنجیره‌ی چند متدی، race ندارد.
            return this.itemsDictionary.Values.Select(draftEntry => draftEntry.Item).ToList().AsReadOnly();
        }

    }
}
