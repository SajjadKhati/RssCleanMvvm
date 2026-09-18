using CompositionRoot.Shared.ClassLibrary.StartupModules.AppFinalizerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.SingletonProviderModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class;
using CompositionRoot.WpfApp.ViewModules;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using View.WpfLibrary.RssCleanMvvm.UiLogicModules.AppearanceSwitcherModule.Class;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.Interface;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;


namespace CompositionRoot.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        private AppPaths appPaths;

        private AppInitializer appInitializer;

        private ViewDisplayer viewDisplayer;

        private AppFinalizer appFinalizer;


        ///////////////////////////////////////////////////////////////////////


        public App()
        {
            this.appPaths = new AppPaths();
            this.appInitializer = new AppInitializer(appPaths);
            this.viewDisplayer = new ViewDisplayer(this.Dispatcher);
        }


        ///////////////////////////////////////////////////////////////////////


        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            /// هندلر App_DispatcherUnhandledException ، زمانی اجرا میشه که خطای مدیریت نشده ای ، در نخ Dispatcher ، یعنی در نخ اصلی منتشر بشه .
            /// این هندلر ، قابلیت جلوگیری از کرش برنامه را داره . یعنی بعد از اجرای این هندلر ، اگه e.Handled = true کنیم ، برنامه و پروسه ، کرش نمیشه .
            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
            /// هندلر AppDomain_UnhandledException ، زمانی اجرا میشه که در هر نخ ای در پروسه ی مان ، خطای مدیریت نشده ای اتفاق بیفته .
            /// این هندلر ، قابلیت جلوگیری از کرش برنامه را نداره .یعنی بعد از اجرای این هندلر ، در اغلب اوقات ، برنامه اتوماتیک بسته میشه .
            /// این هندلر ، در همون نخی که خطا اتفاق افتاد ، اجرا میشه .
            /// بیشترین کاربردِ این هندلر ، معمولا لاگ کردنِ خطا هست .
            /// 
            /// دقت کنید که این هندلر ، تقریبا شاید هیچ وقت خطایی که در شی Task هست را منتشر نمیکنه .
            /// چون شی Task را یا با await میذاریم که پس معمولا در نخ اصلی منتشر میشه و اگه try-cache اش نکرده باشیم ، در هندلرِ App_DispatcherUnhandledException گرفته میشه .
            /// اگر await برای Task مون نذاشته باشیم ، رفتار شی Task این طور هست که خطایی که اتفاق افتاد را اصلا منتشر نمیکنه و در خودش ذخیره میکنه .
            /// یعنی خودِ Task ، یه try-cache ئه داخلی داره و خطا را تا زمانی که کاربر await نکرد یا پروپرتی Result را فراخوانی نکرد یا اعضای دیگه را ، اصلا خطا را منتشر نمیکنه .
            /// برای خطاهای Task هایی که منتشر یا دیده نشده ، باید از رویداد TaskScheduler_UnobservedTaskException استفاده کنیم .
            /// 
            /// دقت کنید اگر در هندلر App_DispatcherUnhandledException ، مقدار e.Handled = true را انجام بدیم ، دیگه خطایی که در نخ اصلی منتشر شد ، -
            /// در همون هندلر (App_DispatcherUnhandledException) مدیریت میشه . پس اون خطا ، دیگه به هندلرِ AppDomain_UnhandledException نمیرسه -
            /// چون قبلا در App_DispatcherUnhandledException ، مدیریت شد .
            AppDomain.CurrentDomain.UnhandledException += this.AppDomain_UnhandledException;
            /// رویداد و هندلر TaskScheduler_UnobservedTaskException ، زمانی اجرا میشه که خطایی در شی Task اتفاق افتاده باشه و مدیریت نشده باشه و -
            /// چون خطاهای مدیریت نشده در Task ، منتشر و بنابراین دیده نمیشن ، در این رویداد ، اون هم فقط زمانی که GC داره اون شی Task را جمع میکنه ، فقط در اون زمان قابل مشاهده هست و-
            /// این هندلر ، در اون زمان اجرا میشه (یعنی فورا بعد از اینکه خطای مدیریت نشده اتفاق افتاد ، منتشر نمیشه و احتمالا ، خیلی بعدا منتشر میشه) .
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            await this.StartAsync();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                this.appFinalizer.Dispose();
            }
            catch (Exception exception)
            {
                
            }
        }


        /// هندلر App_DispatcherUnhandledException ، زمانی اجرا میشه که خطای مدیریت نشده ای ، در نخ Dispatcher ، یعنی در نخ اصلی منتشر بشه .
        /// این هندلر ، قابلیت جلوگیری از کرش برنامه را داره . یعنی بعد از اجرای این هندلر ، اگه e.Handled = true کنیم ، برنامه و پروسه ، کرش نمیشه .
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.viewDisplayer.ShowErrorInMainThread(e.Exception, "خطای مدیریت نشده که در نخ اصلی منتشر شد");

            // جلوی کرش شدن برنامه را توسط خطای مدیریت نشده ، میگیره :
            e.Handled = true;
        }


        /// هندلر AppDomain_UnhandledException ، زمانی اجرا میشه که در هر نخ ای در پروسه ی مان ، خطای مدیریت نشده ای اتفاق بیفته .
        /// این هندلر ، قابلیت جلوگیری از کرش برنامه را نداره .یعنی بعد از اجرای این هندلر ، در اغلب اوقات ، برنامه اتوماتیک بسته میشه .
        /// این هندلر ، در همون نخی که خطا اتفاق افتاد ، اجرا میشه .
        /// بیشترین کاربردِ این هندلر ، معمولا لاگ کردنِ خطا هست .
        /// 
        /// دقت کنید که این هندلر ، تقریبا شاید هیچ وقت خطایی که در شی Task هست را منتشر نمیکنه .
        /// چون شی Task را یا با await میذاریم که پس معمولا در نخ اصلی منتشر میشه و اگه try-cache اش نکرده باشیم ، در هندلرِ App_DispatcherUnhandledException گرفته میشه .
        /// اگر await برای Task مون نذاشته باشیم ، رفتار شی Task این طور هست که خطایی که اتفاق افتاد را اصلا منتشر نمیکنه و در خودش ذخیره میکنه .
        /// یعنی خودِ Task ، یه try-cache ئه داخلی داره و خطا را تا زمانی که کاربر await نکرد یا پروپرتی Result را فراخوانی نکرد یا اعضای دیگه را ، اصلا خطا را منتشر نمیکنه .
        /// برای خطاهای Task هایی که منتشر یا دیده نشده ، باید از رویداد TaskScheduler_UnobservedTaskException استفاده کنیم .
        /// 
        /// دقت کنید اگر در هندلر App_DispatcherUnhandledException ، مقدار e.Handled = true را انجام بدیم ، دیگه خطایی که در نخ اصلی منتشر شد ، -
        /// در همون هندلر (App_DispatcherUnhandledException) مدیریت میشه . پس اون خطا ، دیگه به هندلرِ AppDomain_UnhandledException نمیرسه -
        /// چون قبلا در App_DispatcherUnhandledException ، مدیریت شد .
        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if(e.IsTerminating == true)
            {
                /// لاگ کردن خطا .
                /// بعد از اتامم اجرای این هندلر (AppDomain_UnhandledException) ، برنامه بسته و کرش میشه .
            }
        }


        /// رویداد و هندلر TaskScheduler_UnobservedTaskException ، زمانی اجرا میشه که خطایی در شی Task اتفاق افتاده باشه و مدیریت نشده باشه و -
        /// چون خطاهای مدیریت نشده در Task ، منتشر و بنابراین دیده نمیشن ، در این رویداد ، اون هم فقط زمانی که GC داره اون شی Task را جمع میکنه ، فقط در اون زمان قابل مشاهده هست و-
        /// این هندلر ، در اون زمان اجرا میشه (یعنی فورا بعد از اینکه خطای مدیریت نشده اتفاق افتاد ، منتشر نمیشه و احتمالا ، خیلی بعدا منتشر میشه) .
        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Observed == true)
                return;

            foreach (Exception exception in e.Exception.InnerExceptions)
            {
                this.viewDisplayer.ShowErrorInMainThread(exception, "خطای مدیریت نشده ای که قبلا اتفاق افتاد");
            }
            /// این مجموعه ی خطا (این باری که این هندلر اجرا شد) ، خطاهایش دیده شده و برای دفعه ی بعدی ، بخاطر این خطا ، این هندلر (TaskScheduler_UnobservedTaskException) را اجرا نکن .
            e.SetObserved(); 
        }


        private async Task StartAsync()
        {
            Window waitingWindow = this.viewDisplayer.ShowWaitingWindow();
            try
            {
                string connectionStringJsonFullFilePath = this.GetConnectionStringJsonFullFilePath();
                string connectionString = await this.appInitializer.GetConnectionStringAsync(connectionStringJsonFullFilePath);
                bool isInitialized = await this.appInitializer.InitializeAppAsync(connectionString);
                ///
                SingletonProvider singletonProvider = new SingletonProvider(connectionString, this.appPaths);
                await singletonProvider.InitializeSingletonPropertiesAsync();
                ///
                this.appFinalizer = new AppFinalizer(singletonProvider.RssItemDraftCacheManager);
                ///
                IAppearanceSwitcher appearanceSwitcher = this.CreateAppearanceSwitcher();
                ViewModelCompositionRoot viewModelCompositionRoot = this.GetViewModelCompositionRoot(
                    connectionString, this.appPaths, singletonProvider, appearanceSwitcher);
                MainWindowViewModel mainWindowViewModel = viewModelCompositionRoot.CreateMainWindowViewModel();
                ///
                this.viewDisplayer.ShowMainWindowAndSetupApp(mainWindowViewModel, App.Current);
            }
            catch (Exception exception)
            {
                this.viewDisplayer.ShowErrorInMainThread(exception, "خطا زمان ساخت برنامه");
            }
            finally
            {
                waitingWindow.Close();
            }
        }


        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// این مسیر ، چون شامل مسیر فایل json ای هست که در پروژه ی CompositionRoot.Wpf قرار داره و بنابراین در پروژه ی دیگه (مثلا در Winform) ممکنه تغییر کنه ، -
        /// بخاطر همین ، این متد را در کلاس AppPaths قرار ندادیم .
        /// </summary>
        /// <returns></returns>
        private string GetConnectionStringJsonFullFilePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Resource", "Config", "ConnectionString.json");
        }


        private ViewModelCompositionRoot GetViewModelCompositionRoot(string connectionString, AppPaths appPaths, 
            SingletonProvider singletonProvider, IAppearanceSwitcher appearanceSwitcher)
        {
            return new ViewModelCompositionRoot(
                connectionString, 
                appPaths,
                singletonProvider.RssItemDraftCacheManager,
                singletonProvider.PersistenceCache,
                singletonProvider.RssFeedConfigCache,
                singletonProvider.ImageSettingsWriter,
                singletonProvider.ImageSettingsReader,
                singletonProvider.RssItemRepositoryFactory,
                singletonProvider.RssItemDtoQueryFactory,
                singletonProvider.RssSiteQueryFactory,
                singletonProvider.UnitOfWorkFactory,
                singletonProvider.HttpRequestFactory,
                singletonProvider.RssItemViewModelFactory,
                singletonProvider.ImageChannelCountNormalizerFactory,
                singletonProvider.ImageChannelSequenceNormalizerFactory,
                appearanceSwitcher);
        }


        private IAppearanceSwitcher CreateAppearanceSwitcher()
        {
            SwitchableResourceInfo themeResourceInfo = new SwitchableResourceInfo
            {
                BasePath = "/View.WpfLibrary.RssCleanMvvm;component/GlobalResource/XamlResource/Theme/",
                FileSuffix = "ThemeResource.xaml",
                Index = 2
            };
            ThemeName defaultThemeName = ThemeName.Dark;
            return new AppearanceSwitcher(App.Current.Resources.MergedDictionaries, themeResourceInfo, defaultThemeName);
        }

    }

}
