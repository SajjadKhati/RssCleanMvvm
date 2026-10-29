using WinFormsApplication = System.Windows.Forms.Application;

namespace CompositionRoot.WinForm
{
    /// <summary>
    /// نقطه‌ی شروع برنامه‌ی WinForms .
    ///
    /// معادلِ App.xaml + نقطه‌ی شروع WPF است، ولی چون WinForms فایل XAML و StartupUri ندارد،
    /// تنظیمات اولیه (DPI/VisualStyles/ExceptionMode) و اجرای ApplicationContext اینجا انجام می‌شود .
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            /// ترتیب این سه فراخوانی مهم است و باید قبل از ساخت اولین پنجره انجام شوند .
            WinFormsApplication.SetHighDpiMode(HighDpiMode.SystemAware);
            WinFormsApplication.EnableVisualStyles();
            WinFormsApplication.SetCompatibleTextRenderingDefault(false);

            /// همه‌ی خطاهای مدیریت‌نشده‌ی نخ اصلی را به رویداد Application.ThreadException می‌فرستد
            /// تا در App.cs نمایش داده شوند (معادل DispatcherUnhandledException در WPF) .
            WinFormsApplication.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            /// شی App نقش ApplicationContext را دارد و چرخه‌ی ساخت برنامه را مدیریت می‌کند .
            App app = new();
            WinFormsApplication.Run(app);
        }
    }
}
