namespace CompositionRoot.WIndowsFormApp.ViewModules
{
    /// <summary>
    /// ابزار کمکی برای اجرای یک عمل روی نخ UI .
    /// رویدادهای ViewModel ممکن است از نخ‌های دیگر (مثلا نخ Task ها) منتشر شوند ، -
    /// بنابراین قبل از دست زدن به کنترل‌های WinForms ، باید به نخ UI منتقل شوند .
    /// </summary>
    internal static class UiThread
    {

        public static void Post(Control control, Action action)
        {
            if (control.IsDisposed == true)
                return;

            /// اگر هنوز هندل کنترل ساخته نشده باشد ، BeginInvoke خطا می‌دهد .
            /// در این حالت ، عمل را مستقیم اجرا می‌کنیم (معمولا در زمان ساخت اولیه است) .
            if (control.IsHandleCreated == false)
            {
                action();
                return;
            }

            if (control.InvokeRequired == true)
                control.BeginInvoke(new Action(action));
            else
                action();
        }

    }
}
