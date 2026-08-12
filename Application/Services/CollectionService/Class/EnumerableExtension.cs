namespace Application.Services.CollectionService.Class
{
    /// <summary>
    /// این کلاس را در لایه ی Shared تعریف نشد چون اعضایی داره که به احتمال زیاد ، در لایه های دیگه استفاده نمیشه -
    /// و بیشتر مخصوص همین لایه ی جاری هست .
    /// </summary>
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
