using ViewModel.DataModels.MessengerDataModels.FeedOperationStateMessage.Enum;

namespace ViewModel.DataModels.MessengerDataModels.FeedOperationStateMessage.Class
{
    /// <summary>
    /// پیامی که شروع یا پایان یک عملیات روی یک فید را اطلاع می‌دهد.
    ///
    /// فرستنده: RssFeedViewModel (در ابتدا و finally هر عملیات Load/Download).
    /// گیرنده: RssUserControlViewModel (برای مدیریت counter و CanExecute کامند SaveAll).
    ///
    /// FeedId برای دیباگ/لاگ استفاده می‌شود (مثلاً وقتی counter از تعادل خارج شد،
    /// بدانیم کدام فید مقصر بوده).
    /// </summary>
    public class FeedOperationStateMessage
    {
        public FeedOperationState State { get; }

        public string FeedId { get; }


        //////////////////////////////////////////////////////////////////////////////////


        public FeedOperationStateMessage(FeedOperationState state, string feedId)
        {
            this.State = state;
            this.FeedId = feedId;
        }

    }
}
