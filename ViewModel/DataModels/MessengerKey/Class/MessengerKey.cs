namespace ViewModel.DataModels.MessengerKey.Class
{
    public static class MessengerKey
    {

        public static string Exception_ErrorKey { get; } = "Exception_ErrorKey";

        public static string String_GeneralMessageKey { get; } = "String_GeneralMessageKey";

        /// <summary>
        /// کلید پیام FeedOperationStateMessage، برای هدفِ «وضعیت اجرای عملیات‌ها».
        ///
        /// چرا هدف در نام کلید آمده؟
        /// چون FeedOperationStateMessage ممکن است در آینده برای اهداف دیگری هم استفاده شود
        /// (مثلاً ردیابی پیشرفت، لاگ، ...). با جدا کردن هدف در نام کلید، هر مصرف‌کننده‌ای
        /// که به این هدفِ خاص علاقه دارد، فقط به همین کلید ثبت‌نام می‌کند — نه به کل نوع پیام.
        ///
        /// فرستنده: RssFeedViewModel (در ابتدا و finally هر عملیات Load/Download).
        /// گیرنده: RssUserControlViewModel (برای مدیریت counter و CanExecute کامند SaveAll).
        /// </summary>
        public static string FeedOperationStateMessage_RunningStateKey { get; } = "FeedOperationStateMessage_RunningStateKey";

    }
}
