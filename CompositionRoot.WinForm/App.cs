using CompositionRoot.Shared.ClassLibrary.StartupModules.AppFinalizerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.SingletonProviderModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class;
using CompositionRoot.WinForm.ViewModules;
using View.WinFormLibrary.RssCleanMvvm.AppResource.LanguageResource.Class;
using View.WinFormLibrary.RssCleanMvvm.Elements.Windows.MainWindowView;
using View.WinFormLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView;
using View.WinFormLibrary.RssCleanMvvm.UiLogicModules.ConfirmationDialog.Class;
using View.WinFormLibrary.RssCleanMvvm.UiLogicModules.ResourceSwitcherModule.Class;
using View.WinFormLibrary.RssCleanMvvm.UiLogicModules.WaitingDialogModule.Class;
using ViewModel.Contracts.ViewContracts.ConfirmationDialog.Interface;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.Interface;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.Interface;
using ViewModel.Contracts.ViewContracts.WaitingDialog.Interface;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using WinFormsApplication = System.Windows.Forms.Application;

namespace CompositionRoot.WinForm
{
    /// <summary>
    /// معادلِ کلاس <c>App</c> در نسخه‌ی WPF .
    ///
    /// چون در WinForms کلاس Application قابل ارث‌بری نیست، از <see cref="ApplicationContext"/>
    /// استفاده می‌کنیم؛ نقشِ همان Application است :
    ///   • ریشه‌ی ترکیب (Composition Root) و چرخه‌ی ساخت برنامه
    ///   • ثبت هندلرهای خطاهای مدیریت‌نشده
    ///   • ساخت ViewModel و وصل کردن آن به پنجره‌ی اصلی
    ///
    /// نکته‌ی مهم درباره‌ی ReactiveUI/CommunityToolkit :
    /// ViewModel ها همان ViewModel های نسخه‌ی WPF اند (بدون هیچ تغییری) و
    /// کامندها ICommand/IRelayCommand/IAsyncRelayCommand هستند . پس این پروژه فقط
    /// آن‌ها را می‌سازد و به View می‌دهد؛ هیچ منطق تجاری‌ای اینجا نیست .
    /// </summary>
    internal sealed class App : ApplicationContext
    {
        private readonly AppPaths appPaths;

        private readonly AppInitializer appInitializer;

        private readonly ViewDisplayer viewDisplayer;

        private readonly WaitingWindow waitingWindow;

        private AppFinalizer? appFinalizer;

        private bool isStarted;


        ///////////////////////////////////////////////////////////////////////


        public App()
        {
            this.appPaths = new AppPaths();
            this.appInitializer = new AppInitializer(this.appPaths);
            this.viewDisplayer = new ViewDisplayer();

            /// هندلر Application.ThreadException ، معادل App_DispatcherUnhandledException در WPF است و
            /// فقط خطاهای مدیریت‌نشده‌ی نخ اصلی (نخ UI) را دریافت می‌کند . این هندلر می‌تواند از کرش
            /// جلوگیری کند (چون پس از آن، برنامه به کار خود ادامه می‌دهد) .
            WinFormsApplication.ThreadException += this.Application_ThreadException;

            /// هندلر AppDomain.UnhandledException ، خطاهای مدیریت‌نشده‌ی هر نخی را دریافت می‌کند، ولی
            /// نمی‌تواند از کرش شدن برنامه جلوگیری کند و بیشتر برای لاگ کردن مناسب است .
            AppDomain.CurrentDomain.UnhandledException += this.AppDomain_UnhandledException;

            /// خطاهای مدیریت‌نشده‌ی Task ها که هیچ‌وقت مشاهده نشده‌اند، فقط هنگام جمع‌آوری شی Task توسط GC،
            /// از طریق این رویداد قابل مشاهده هستند .
            TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;

            /// برای Dispose کردن منابعی که در طول عمر برنامه ساخته شده‌اند (مثل کش موقت) .
            WinFormsApplication.ApplicationExit += this.Application_ApplicationExit;

            /// پنجره‌ی انتظار، قبل از InitializeApp نمایش داده می‌شود (همان الگوی WPF) .
            this.waitingWindow = this.viewDisplayer.ShowWaitingWindow();

            /// تا زمانی که پنجره‌ی اصلی ساخته نشود، پنجره‌ی انتظار MainForm این ApplicationContext است .
            this.MainForm = this.waitingWindow;

            /// شروع کار را به پیام‌لوپ WinForms می‌سپاریم تا WindowsFormsSynchronizationContext
            /// حتماً نصب شده باشد . (ViewModel ها در سازنده‌ی RssUserControlViewModel به
            /// SynchronizationContext.Current نیاز دارند و در غیر این صورت استثنا می‌دهند)
            this.waitingWindow.BeginInvoke(new Action(this.StartOnUiThread));
        }


        ///////////////////////////////////////////////////////////////////////


        private async void StartOnUiThread()
        {
            await this.StartAsync();
        }


        /// <summary>
        /// همان مسیر StartAsync در نسخه‌ی WPF .
        ///
        /// تفاوتِ عمدی: بجای AppearanceSwitcher/LanguageSwitcher واقعی، نسخه‌ی NoOp آن‌ها
        /// داده می‌شود؛ چون تغییر تم/زبان در این نسخه وجود ندارد . بقیه‌ی جریان (خواندن
        /// ConnectionString، ساخت دیتابیس، SingletonProvider، ViewModelCompositionRoot و
        /// ساخت MainWindowViewModel) دقیقاً مثل WPF است .
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

                SingletonProvider singletonProvider = new(connectionString, this.appPaths);
                await singletonProvider.InitializeSingletonPropertiesAsync();

                this.appFinalizer = new AppFinalizer(new List<IDisposable> { singletonProvider.RssItemDraftCacheManager });

                /// پنجره‌ی اصلی را قبل از دیالوگ‌ها می‌سازیم تا WaitingDialog بتواند Owner داشته باشد .
                MainWindow mainWindow = new();

                IAppearanceSwitcher appearanceSwitcher = new NoOpAppearanceSwitcher();
                ILanguageSwitcher languageSwitcher = new NoOpLanguageSwitcher();
                IWaitingDialog waitingDialog = new WaitingDialog(mainWindow);
                IConfirmationDialog confirmationDialog = new ConfirmationDialog();

                ViewModelCompositionRoot viewModelCompositionRoot = this.GetViewModelCompositionRoot(
                    connectionString, singletonProvider, appearanceSwitcher, languageSwitcher,
                    waitingDialog, confirmationDialog);

                MainWindowViewModel mainWindowViewModel = viewModelCompositionRoot.CreateMainWindowViewModel();

                this.viewDisplayer.ShowMainWindowAndSetup(mainWindow, mainWindowViewModel, this);
            }
            catch (Exception exception)
            {
                this.viewDisplayer.ShowErrorInMainThread(exception, PersianLanguageResource.StartupErrorCaption);
            }
            finally
            {
                if (this.waitingWindow.IsDisposed == false)
                    this.waitingWindow.CloseFromOwner();
            }
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// این مسیر، چون شامل مسیر فایل json ای است که در پروژه‌ی CompositionRoot.WinForm قرار دارد
        /// و بنابراین در پروژه‌ی دیگر (مثلاً در WPF) ممکن است تغییر کند، در کلاس AppPaths قرار داده نشده است .
        /// </summary>
        private string GetConnectionStringJsonFullFilePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Resource", "Config", "ConnectionString.json");
        }


        private ViewModelCompositionRoot GetViewModelCompositionRoot(string connectionString,
            SingletonProvider singletonProvider, IAppearanceSwitcher appearanceSwitcher,
            ILanguageSwitcher languageSwitcher, IWaitingDialog waitingDialog, IConfirmationDialog confirmationDialog)
        {
            return new ViewModelCompositionRoot(
                connectionString,
                this.appPaths,
                singletonProvider.RssItemDraftCacheManager,
                singletonProvider.PersistenceCache,
                singletonProvider.SharedPersistenceCacheLock,
                singletonProvider.RssFeedConfigCache,
                appearanceSwitcher,
                languageSwitcher,
                waitingDialog,
                confirmationDialog);
        }


        ///////////////////////////////////////////////////////////////////////


        private void Application_ThreadException(object? sender, ThreadExceptionEventArgs e)
        {
            this.viewDisplayer.ShowErrorInMainThread(e.Exception,
                PersianLanguageResource.UnhandledMainThreadErrorCaption);
        }


        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            /// این هندلر نمی‌تواند از کرش جلوگیری کند؛ فقط برای لاگ کردن مناسب است .
            /// (مثل نسخه‌ی WPF، از نمایش MessageBox در نخِ خطا خودداری می‌کنیم تا deadlock نشود .)
            if (e.IsTerminating == true && e.ExceptionObject is Exception exception)
                System.Diagnostics.Debug.WriteLine(exception);
        }


        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Observed == true)
                return;

            foreach (Exception exception in e.Exception.InnerExceptions)
            {
                this.viewDisplayer.ShowErrorInMainThread(exception,
                    PersianLanguageResource.UnobservedTaskErrorCaption);
            }

            /// این مجموعه خطا، دیده‌شده تلقی شود تا برای همان خطا دوباره این هندلر اجرا نشود .
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
                /// در زمان خروج، خطای Dispose را نادیده می‌گیریم تا برنامه به‌درستی بسته شود .
            }
        }

    }
}
