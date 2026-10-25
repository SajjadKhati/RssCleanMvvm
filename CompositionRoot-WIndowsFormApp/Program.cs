using WinFormsApplication = System.Windows.Forms.Application;

namespace CompositionRoot.WIndowsFormApp
{
    internal static class Program
    {
        /// <summary>
        /// نقطه شروع برنامه WinForms .
        /// </summary>
        [STAThread]
        private static void Main()
        {
            /// تنظیمات DPI و ظاهر پیش‌فرض WinForms .
            /// ترتیب فراخوانی این متدها مهم است و باید قبل از ساخت اولین پنجره انجام شوند .
            WinFormsApplication.SetHighDpiMode(HighDpiMode.SystemAware);
            WinFormsApplication.EnableVisualStyles();
            WinFormsApplication.SetCompatibleTextRenderingDefault(false);

            /// همه‌ی خطاهای مدیریت‌نشده‌ی نخ اصلی را به رویداد Application.ThreadException می‌فرستد .
            WinFormsApplication.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            /// شی App ، نقش ApplicationContext را دارد و چرخه‌ی ساخت برنامه را مدیریت می‌کند .
            App app = new App();
            WinFormsApplication.Run(app);
        }
    }
}
