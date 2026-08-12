using Application.InfrastructureContracts.CacheProject.Cache.Interfaces;
using System.Collections.Concurrent;

namespace Infra.CacheService.CacheServices.Class
{
    public class Cache<TKey, TItem> : ICache<TKey, TItem>
        where TKey : notnull
    {

        protected ConcurrentDictionary<TKey, TItem> itemsDictionary;


        ////////////////////////////////////////////////


        public Cache(int collectionCapacity = -1)
        {
            collectionCapacity = (collectionCapacity <= 0) ? 31 : collectionCapacity;
            this.itemsDictionary = new ConcurrentDictionary<TKey, TItem>(-1, collectionCapacity);
        }


        ////////////////////////////////////////////////


        public bool ContainsKey(TKey key)
        {
            return this.itemsDictionary.ContainsKey(key);
        }


        public TItem Get(TKey key)
        {
            return this.itemsDictionary[key];
        }


        public IReadOnlyList<TItem> GetAll()
        {
            return this.itemsDictionary.Values.ToList().AsReadOnly();
        }


        public TItem GetOrAdd(TKey key, Func<TKey, TItem> valueFactory)
        {
            return this.itemsDictionary.GetOrAdd(key, valueFactory);
        }


        public void Set(TKey key, TItem value)
        {
            this.itemsDictionary[key] = value;
        }


        public bool Remove(TKey key)
        {
            return this.itemsDictionary.Remove(key, out TItem item);
        }


        public void Clear()
        {
            this.itemsDictionary.Clear();
        }

    }
}
