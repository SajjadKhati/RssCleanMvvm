using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CompositionRoot.WpfApp.ViewModules;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;


namespace CompositionRoot.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        private AppInitializer appInitializer;

        private ViewDisplayer viewDisplayer;


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
                IReadOnlyDictionary<Uri, RssFeedConfig> rssFeedDictionary = 
                    await this.appInitializer.GetRssFeedConfigDictionaryAsync(connectionString);
                ///
                ViewModelCompositionRoot viewModelCompositionRoot = new ViewModelCompositionRoot(connectionString, rssFeedDictionary);
                MainWindowViewModel mainWindowViewModel = viewModelCompositionRoot.CreateMainWindowViewModel();
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

    }

}
