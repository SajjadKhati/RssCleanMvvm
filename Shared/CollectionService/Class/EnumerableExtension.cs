namespace Shared.CollectionService.Class
{
    public static class EnumerableExtension
    {

        public static IReadOnlyList<T> AddToFirstItem<T>(this IEnumerable<T> lastItems, T firstItem)
        {
            List<T> targetList = new List<T>();
            targetList.Add(firstItem);
            targetList.AddRange(lastItems);
            return targetList.AsReadOnly();
        }

    }
}
