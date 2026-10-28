namespace ViewModel.DataModels.MessengerDataModels.FeedOperationStateMessage.Enum
{
    /// <summary>
    /// وضعیت یک عملیات (Load یا Download) روی یک فید.
    /// </summary>
    public enum FeedOperationState
    {
        /// <summary>عملیات شروع شد.</summary>
        Started = 1,

        /// <summary>عملیات پایان یافت (چه موفق، چه لغو، چه خطا).</summary>
        Ended
    }
}
