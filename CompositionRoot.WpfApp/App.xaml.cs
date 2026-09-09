using CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.SingletonProviderModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class;
using CompositionRoot.WpfApp.ViewModules;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppFinalizerModule.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using System.Data;


namespace CompositionRoot.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        private AppInitializer appInitializer;

        private ViewDisplayer viewDisplayer;

        private AppFinalizer appFinalizer;


        ///////////////////////////////////////////////////////////////////////


        public App()
        {
            this.appInitializer = new AppInitializer();
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
                SingletonProvider singletonProvider = new SingletonProvider(connectionString);
                await singletonProvider.InitializeComponentsAsync();
                ///
                this.appFinalizer = new AppFinalizer(singletonProvider.RssItemDraftCacheManager);
                ///
                ViewModelCompositionRoot viewModelCompositionRoot = this.GetViewModelCompositionRoot(connectionString, singletonProvider);
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


        private string GetConnectionStringJsonFullFilePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Resource", "Config", "ConnectionString.json");
        }


        private ViewModelCompositionRoot GetViewModelCompositionRoot(string connectionString, SingletonProvider singletonProvider)
        {
            return new ViewModelCompositionRoot(
                connectionString, 
                singletonProvider.RssItemDraftCacheManager,
                singletonProvider.PersistenceCache,
                singletonProvider.RssFeedConfigCache,
                singletonProvider.RssItemRepositoryFactory,
                singletonProvider.RssItemDtoQueryFactory,
                singletonProvider.RssSiteQueryFactory,
                singletonProvider.UnitOfWorkFactory,
                singletonProvider.HttpRequestFactory,
                singletonProvider.ImageChannelCountNormalizerFactory,
                singletonProvider.ImageChannelSequenceNormalizerFactory);
        }

    }

}
