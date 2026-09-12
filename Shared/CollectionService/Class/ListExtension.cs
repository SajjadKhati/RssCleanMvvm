namespace Shared.CollectionService.Class
{
    public static class ListExtension
    {

        /// <summary>
        /// یک کالکشن (insertingCollection) را به mainList که ایندکس اش از mainCollectionStartingIndex شرووع میشود را اضافه میکند .
        /// </summary>
        /// <typeparam name="T">نوع آیتم</typeparam>
        /// <param name="mainList">لیست اصلی ای که به آن اضافه میشود .</param>
        /// <param name="insertingCollection">کالکشن ای که قرار هست آیتم هایش به mainList اضافه شود .</param>
        /// <param name="mainCollectionStartingIndex">ایندکس شروع که از آن ایندکس به بعد در mainList ، شروع به اضافه کردنِ کالکشنِ insertingCollection میکند .
        /// اگر مقدارش منفی باشد ، به انتهای لیست mainList افزوده میشود .
        /// اگر مقدارش 0 باشد ، به ابتدای لیستِ mainList افزوده میشود .</param>
        public static void InsertRangeAt<T>(this IList<T> mainList, ICollection<T> insertingCollection, int mainCollectionStartingIndex = -1)
        {
            ArgumentNullException.ThrowIfNull(insertingCollection);
            if (insertingCollection.Count < 1)
                return;
            if(mainCollectionStartingIndex < 0)
            {
                mainCollectionStartingIndex = mainList.Count;
            }

            int index = mainCollectionStartingIndex;
            foreach (T insertingItem in insertingCollection)
            {
                mainList.Insert(index++, insertingItem);
            }
        }

    }
}
