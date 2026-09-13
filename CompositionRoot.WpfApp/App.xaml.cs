using CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.SingletonProviderModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class;
using CompositionRoot.WpfApp.ViewModules;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppFinalizerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
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

            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
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


        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.viewDisplayer.ShowErrorInMainThread(e.Exception);

            // جلوی کرش شدن برنامه را توسط خطای مدیریت نشده ، میگیره :
            e.Handled = true;
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
                ViewModelCompositionRoot viewModelCompositionRoot = this.GetViewModelCompositionRoot(connectionString, this.appPaths, singletonProvider);
                MainWindowViewModel mainWindowViewModel = viewModelCompositionRoot.CreateMainWindowViewModel();
                ///
                this.viewDisplayer.ShowMainWindow(mainWindowViewModel);
            }
            catch (Exception exception)
            {
                this.viewDisplayer.ShowErrorInMainThread(exception);
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


        private ViewModelCompositionRoot GetViewModelCompositionRoot(string connectionString, AppPaths appPaths, SingletonProvider singletonProvider)
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
                singletonProvider.ImageChannelSequenceNormalizerFactory);
        }

    }

}
