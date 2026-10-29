namespace View.WinFormLibrary.RssCleanMvvm.AppResource.LanguageResource.Class
{
    /// <summary>
    /// معادلِ <c>PersianLanguageResource.xaml</c> در نسخه‌ی WPF .
    ///
    /// در نسخه‌ی WinForms ، قابلیت تغییر زبان وجود ندارد (طبق درخواست) ، بنابراین :
    ///   • فقط یک فایل زبان (فارسی) داریم .
    ///   • به‌جای ResourceDictionary ، از یک کلاس static با رشته‌های const استفاده می‌کنیم .
    ///
    /// مزیت این کار همان مزیت ResourceDictionary است : متن‌ها در یک جا متمرکزند و
    /// کنترل‌ها (View) هیچ متن سخت‌کدشده‌ای ندارند .
    /// </summary>
    public static class PersianLanguageResource
    {
        // ===== پنجره‌ی اصلی =====

        public const string ApplicationTitle = "خبر خوان طلوع";

        // ===== منوی اصلی =====

        public const string NewsFeedMenu = "خبر خوان";

        public const string GetNewNews = "دریافت اخبار جدید";

        public const string GetNewNewsToolTip = "به روزرسانی اخبار جدید برای موضوع خبری انتخاب شده";

        public const string CancelCurrentDownload = "لغو دانلود جاری";

        public const string CancelOperationToolTip = "لغو عملیات دانلود برای موضوع خبری انتخاب شده";

        public const string SaveAll = "ذخیره همه";

        public const string SaveAllToolTip = "ذخیره همه ی آیتم های ذخیره نشده برای تمام موضوعات خبری";

        // ===== هدرِ انتخاب خبرگزاری / دسته‌ی خبری =====

        public const string NewsSite = "سایت خبری :";

        public const string SelectNewsAgency = "خبرگزاری را انتخاب کنید";

        public const string NewsCategory = "دسته ی خبری : ";

        public const string SelectNewsCategory = "موضوع خبری را انتخاب کنید";

        // ===== آیتم خبر =====

        public const string EditMode = "حالت ویرایش";

        public const string ReadMode = "حالت خواندن";

        public const string ConvertToGrayScale = "تبدیل به سیاه و سفید";

        public const string Save = "ذخیره";

        public const string CategoryFormat = "در دسته بندی : {0}";

        // ===== پنجره‌ی انتظار =====

        public const string Loading = "در حال بارگذاری ...";

        public const string Saving = "در حال ذخیره سازی ...";

        // ===== خطاها =====

        public const string ErrorCaption = "خطا";

        public const string StartupErrorCaption = "خطا زمان ساخت برنامه";

        public const string UnhandledMainThreadErrorCaption = "خطای مدیریت‌نشده که در نخ اصلی منتشر شد";

        public const string UnhandledErrorCaption = "خطای مدیریت‌نشده";

        public const string UnobservedTaskErrorCaption = "خطای مدیریت‌نشده‌ای که قبلا اتفاق افتاد";

        public const string NoImage = "بدون تصویر";
    }
}
