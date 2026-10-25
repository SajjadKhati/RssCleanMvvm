using CompositionRoot.Shared.ClassLibrary.StartupModules.AppFinalizerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.SingletonProviderModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class;
using CompositionRoot.WIndowsFormApp.UiLogicModules.ResourceSwitcherModule;
using CompositionRoot.WIndowsFormApp.ViewModules;
using System.IO;
using WinFormsApplication = System.Windows.Forms.Application;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.Interface;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.Interface;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;

namespace CompositionRoot.WIndowsFormApp
{
    /// <summary>
    /// معادل کلاس App در نسخه‌ی WPF .
    /// چون در WinForms کلاس Application قابل ارث‌بری نیست ، از ApplicationContext استفاده می‌کنیم .
    /// </summary>
    internal sealed class App : ApplicationContext
    {

        private readonly AppPaths appPaths;

        private readonly AppInitializer appInitializer;

        private readonly ViewDisplayer viewDisplayer;

        private readonly WaitingForm waitingForm;

        private AppFinalizer? appFinalizer;

        private bool isStarted;


        ///////////////////////////////////////////////////////////////////////


        public App()
        {
            this.appPaths = new AppPaths();
            this.appInitializer = new AppInitializer(this.appPaths);
            this.viewDisplayer = new ViewDisplayer();

            /// هندلر Application.ThreadException ، معادل App_DispatcherUnhandledException در WPF است و -
            /// فقط خطاهای مدیریت‌نشده‌ی نخ اصلی (نخ UI) را دریافت می‌کند .
            WinFormsApplication.ThreadException += this.Application_ThreadException;
            /// هندلر AppDomain.UnhandledException ، خطاهای مدیریت‌نشده‌ی هر نخی را دریافت می‌کند ، ولی -
            /// نمی‌تواند از کرش شدن برنامه جلوگیری کند و بیشتر برای لاگ کردن مناسب است .
            AppDomain.CurrentDomain.UnhandledException += this.AppDomain_UnhandledException;
            /// خطاهای مدیریت‌نشده‌ی Task ها که هیچ‌وقت مشاهده نشده‌اند ، فقط هنگام جمع‌آوری شی Task توسط GC ، -
            /// از طریق این رویداد قابل مشاهده هستند .
            TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;
            /// برای Dispose کردن منابعی که در طول عمر برنامه ساخته شده‌اند (مثل کش موقت) .
            WinFormsApplication.ApplicationExit += this.Application_ApplicationExit;

            this.waitingForm = new WaitingForm();
            this.viewDisplayer.AttachUiAnchor(this.waitingForm);
            this.waitingForm.Shown += this.WaitingForm_Shown;

            /// تا زمانی که پنجره‌ی اصلی ساخته نشود ، پنجره‌ی انتظار ، MainForm شی ApplicationContext است .
            /// به این ترتیب با بسته شدن آن (در صورت خطا) ، پیام‌لوپ برنامه هم خاتمه پیدا می‌کند .
            this.MainForm = this.waitingForm;
            this.waitingForm.Show();
        }


        ///////////////////////////////////////////////////////////////////////


        private void WaitingForm_Shown(object? sender, EventArgs e)
        {
            /// شروع کار را به پیام‌لوپ WinForms می‌سپاریم تا WindowsFormsSynchronizationContext -
            /// حتما نصب شده باشد و ادامه‌ی await ها روی نخ UI اجرا شوند .
            this.waitingForm.BeginInvoke(new Action(this.StartOnUiThread));
        }


        private async void StartOnUiThread()
        {
            await this.StartAsync();
        }


        /// <summary>
        /// همان مسیر StartAsync در نسخه‌ی WPF ، با این تفاوت که بجای AppearanceSwitcher و LanguageSwitcher ، -
        /// نسخه‌های NoOp آن‌ها به ViewModelCompositionRoot داده می‌شود ؛ چون تغییر Theme و Language -
        /// در نسخه‌ی WinForms وجود ندارد .
        /// </summary>
        private async Task StartAsync()
        {
            if (this.isStarted == true)
                return;
            this.isStarted = true;

            try
            {
                string connectionStringJsonFullFilePath = this.GetConnectionStringJsonFullFilePath();
                string connectionString = await this.appInitializer.GetConnectionStringAsync(connectionStringJsonFullFilePath);
                await this.appInitializer.InitializeAppAsync(connectionString);
                ///
                SingletonProvider singletonProvider = new SingletonProvider(connectionString, this.appPaths);
                await singletonProvider.InitializeSingletonPropertiesAsync();
                ///
                this.appFinalizer = new AppFinalizer(new List<IDisposable> { singletonProvider.RssItemDraftCacheManager });
                ///
                IAppearanceSwitcher appearanceSwitcher = new NoOpAppearanceSwitcher();
                ILanguageSwitcher languageSwitcher = new NoOpLanguageSwitcher();
                ViewModelCompositionRoot viewModelCompositionRoot = new ViewModelCompositionRoot(
                    connectionString,
                    this.appPaths,
                    singletonProvider.RssItemDraftCacheManager,
                    singletonProvider.PersistenceCache,
                    singletonProvider.SharedPersistenceCacheLock,
                    singletonProvider.RssFeedConfigCache,
                    appearanceSwitcher,
                    languageSwitcher);
                MainWindowViewModel mainWindowViewModel = viewModelCompositionRoot.CreateMainWindowViewModel();
                ///
                this.viewDisplayer.ShowMainWindowAndSetupApp(mainWindowViewModel, this);
            }
            catch (Exception exception)
            {
                this.viewDisplayer.ShowErrorInMainThread(exception, "خطا زمان ساخت برنامه");
            }
            finally
            {
                if (this.waitingForm.IsDisposed == false)
                    this.waitingForm.Close();
            }
        }


        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// این مسیر ، چون شامل مسیر فایل json ای هست که در پروژه‌ی CompositionRoot.WIndowsFormApp قرار دارد و -
        /// بنابراین در پروژه‌ی دیگر (مثلا در WPF) ممکن است تغییر کند ، بخاطر همین ، این متد را در کلاس AppPaths قرار ندادیم .
        /// </summary>
        private string GetConnectionStringJsonFullFilePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Resource", "Config", "ConnectionString.json");
        }


        ///////////////////////////////////////////////////////////////////////////


        private void Application_ThreadException(object? sender, ThreadExceptionEventArgs e)
        {
            this.viewDisplayer.ShowErrorInMainThread(e.Exception, "خطای مدیریت‌نشده که در نخ اصلی منتشر شد");
        }


        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                this.viewDisplayer.ShowErrorInMainThread(exception, "خطای مدیریت‌نشده");
        }


        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Observed == true)
                return;

            foreach (Exception exception in e.Exception.InnerExceptions)
            {
                this.viewDisplayer.ShowErrorInMainThread(exception, "خطای مدیریت‌نشده‌ای که قبلا اتفاق افتاد");
            }
            /// این مجموعه‌ی خطا (این باری که این هندلر اجرا شد) ، خطاهایش دیده شده و برای دفعه‌ی بعدی ، -
            /// بخاطر این خطا ، این هندلر را اجرا نکن .
            e.SetObserved();
        }


        private void Application_ApplicationExit(object? sender, EventArgs e)
        {
            try
            {
                this.appFinalizer?.Dispose();
            }
            catch (Exception)
            {
                /// در زمان خروج برنامه ، خطای Dispose را نادیده می‌گیریم تا برنامه به درستی بسته شود .
            }
        }

    }
}
