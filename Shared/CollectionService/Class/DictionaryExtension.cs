namespace Shared.CollectionService.Class
{
    public static class DictionaryExtension
    {

        public static void AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> target,
            IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var pair in source)
                target.Add(pair.Key, pair.Value);
        }

    }
}
