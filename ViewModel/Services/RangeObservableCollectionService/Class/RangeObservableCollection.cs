using Shared.CollectionService.Class;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ViewModel.Services.RangeObservableCollectionService.Class
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {

        public RangeObservableCollection() : base()
        {
        }

        public RangeObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public RangeObservableCollection(List<T> list) : base(list)
        {
        }


        ////////////////////////////////////////////////////////////////////


        /// <summary>
        /// کالکشنِ (collection) را به کالکشن شی جاری (RangeObservableCollection)  که ایندکس اش از startingIndex شرووع میشود را اضافه میکند .
        /// </summary>
        /// <param name="collection">کالکشن ای که قرار هست آیتم هایش به شی جاری (RangeObservableCollection) اضافه شود .</param>
        /// <param name="startingIndex">ایندکس شروع که از آن ایندکس به بعد ، در شی جاری (RangeObservableCollection) ، شروع به اضافه کردنِ کالکشنِ collection میکند .
        /// اگر مقدارش منفی باشد ، به انتهای کالکشن جاری (RangeObservableCollection) افزوده میشود .
        /// اگر مقدارش 0 باشد ، به ابتدای کالکشن جاری (RangeObservableCollection) افزوده میشود </param>
        public void InsertRangeWithSingleNotification(ICollection<T> collection, int startingIndex = -1)
        {
            if (collection == null || collection.Count < 1)
                return;
            this.Items.InsertRangeAt(collection, startingIndex);
            NotifyCollectionChangedAction resetNotifyCollectionChangedAction = NotifyCollectionChangedAction.Reset;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(resetNotifyCollectionChangedAction));
        }

    }
}
