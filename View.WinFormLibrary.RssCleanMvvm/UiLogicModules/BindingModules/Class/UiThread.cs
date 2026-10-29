namespace View.WinFormLibrary.RssCleanMvvm.UiLogicModules.BindingModules.Class
{
    /// <summary>
    /// ابزار کمکی برای اجرای یک عمل روی نخ UI .
    ///
    /// چرا لازم است؟
    /// ViewModel ها (مثلاً RssFeedViewModel) بعضی پیام‌ها/تغییرها را پس از await و روی
    /// SynchronizationContext نخ UI منتشر می‌کنند، ولی برخی رویدادها (مثل
    /// TaskScheduler.UnobservedTaskException یا خطاهای پس‌زمینه) ممکن است از نخ دیگری برسند .
    /// دست‌زدن به کنترل‌های WinForms از نخ غیر UI باعث InvalidOperationException می‌شود .
    /// </summary>
    public static class UiThread
    {
        public static void Post(Control? control, Action action)
        {
            if (control == null || action == null)
                return;

            if (control.IsDisposed == true)
                return;

            void RunSafely()
            {
                if (control.IsDisposed == false)
                    action();
            }

            /// اگر هندل ساخته نشده باشد (مثلاً در زمان ساخت اولیه‌ی View)، BeginInvoke خطا می‌دهد .
            if (control.IsHandleCreated == false)
            {
                RunSafely();
                return;
            }

            if (control.InvokeRequired == true)
            {
                try
                {
                    control.BeginInvoke(new Action(RunSafely));
                }
                catch (InvalidOperationException)
                {
                    /// بین بررسی IsHandleCreated و BeginInvoke ممکن است هندل نابود شود .
                    /// (ObjectDisposedException هم زیرمجموعه‌ی همین نوع است .)
                }
                return;
            }

            RunSafely();
        }
    }
}
