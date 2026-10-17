namespace Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces
{
    public interface ICache<TKey, TItem> where TKey : notnull
    {

        int Count();

        bool ContainsKey(TKey key);

        IReadOnlyCollection<TKey> GetKeys();

        bool TryGetValue(TKey key, out TItem value);

        IReadOnlyList<TItem> GetAllValuesCopy();

        IReadOnlyDictionary<TKey, TItem> GetAllKeyValues();

        IReadOnlyDictionary<TKey, TItem> GetAllKeyValuesCopy();

        TItem GetOrAdd(TKey key, TItem value);

        TItem GetOrAdd(TKey key, Func<TKey, TItem> addValueFactory);

        TItem AddOrUpdate(TKey key, TItem value);

        TItem AddOrUpdate(TKey key, Func<TKey, TItem> addValueFactory, Func<TKey, TItem, TItem> updateValueFactory);

        void Set(TKey key, TItem value);

        bool TryRemove(TKey key);

        bool TryRemove(TKey key, out TItem value);

        void Clear();

    }
}
