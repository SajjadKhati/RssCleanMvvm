using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Infra.CacheService.CacheBaseServices.CacheServices.Class
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


        public int Count()
        {
            return this.itemsDictionary.Count;
        }


        public bool ContainsKey(TKey key)
        {
            return this.itemsDictionary.ContainsKey(key);
        }


        public ICollection<TKey> GetKeys()
        {
            return this.itemsDictionary.Keys;
        }


        public TItem Get(TKey key)
        {
            return this.itemsDictionary[key];
        }


        public IReadOnlyList<TItem> GetAll()
        {
            return this.itemsDictionary.Values.ToList().AsReadOnly();
        }


        public IReadOnlyDictionary<TKey, TItem> GetAllKeyValuesSnapshot()
        {
            return new ReadOnlyDictionary<TKey, TItem>(this.itemsDictionary);
        }


        public TItem GetOrAdd(TKey key, TItem value)
        {
            return this.itemsDictionary.GetOrAdd(key, value);
        }


        public TItem GetOrAdd(TKey key, Func<TKey, TItem> addValueFactory)
        {
            return this.itemsDictionary.GetOrAdd(key, addValueFactory);
        }


        public TItem AddOrUpdate(TKey key, TItem value)
        {
            this.itemsDictionary[key] = value;
            return this.itemsDictionary[key];
        }


        public TItem AddOrUpdate(TKey key, Func<TKey, TItem> addValueFactory, Func<TKey, TItem, TItem> updateValueFactory)
        {
            return this.itemsDictionary.AddOrUpdate(key, addValueFactory, updateValueFactory);
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
