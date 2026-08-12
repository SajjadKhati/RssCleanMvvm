namespace Application.InfrastructureContracts.CacheProject.Cache.Interfaces
{
    public interface ICache<TKey, TItem> where TKey : notnull
    {
        bool ContainsKey(TKey key);

        TItem Get(TKey key);

        IReadOnlyList<TItem> GetAll();

        TItem GetOrAdd(TKey key, Func<TKey, TItem> valueFactory);

        void Set(TKey key, TItem value);

        bool Remove(TKey key);

        void Clear();

    }
}
