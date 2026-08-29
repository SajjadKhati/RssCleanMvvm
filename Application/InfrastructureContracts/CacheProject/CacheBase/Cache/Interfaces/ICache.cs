namespace Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces
{
    public interface ICache<TKey, TItem> where TKey : notnull
    {

        int Count();

        bool ContainsKey(TKey key);

        ICollection<TKey> GetKeys();

        TItem Get(TKey key);

        IReadOnlyList<TItem> GetAll();

        IReadOnlyDictionary<TKey, TItem> GetAllKeyValuesSnapshot();

        TItem GetOrAdd(TKey key, TItem value);

        TItem GetOrAdd(TKey key, Func<TKey, TItem> addValueFactory);

        TItem AddOrUpdate(TKey key, TItem value);

        TItem AddOrUpdate(TKey key, Func<TKey, TItem> addValueFactory, Func<TKey, TItem, TItem> updateValueFactory);

        void Set(TKey key, TItem value);

        bool Remove(TKey key);

        void Clear();

    }
}
