using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class;
using CompositionRoot.Shared.ClassLibrary.StartupModules.ViewModelCompositionRootModule.Class;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using View.WpfLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using MainWindowView = View.WpfLibrary.RssCleanMvvm.Elements.Windows.MainWindowView;


namespace CompositionRoot.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        private AppInitializer appInitializer;


        ///////////////////////////////////////////////////////////////////////


        public App()
        {
            this.appInitializer = new AppInitializer();
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
            this.ShowErrorInMainThread(e.Exception);

            // جلوی کرش شدن برنامه را توسط خطای مدیریت نشده ، میگیره :
            e.Handled = true;
        }


        private async Task StartAsync()
        {
            Window waitingWindow = this.ShowWaitingWindow();
            try
            {
                string connectionStringJsonFullFilePath = this.GetConnectionStringJsonFullFilePath();
                string connectionString = await this.appInitializer.GetConnectionStringAsync(connectionStringJsonFullFilePath);
                bool isInitialized = await this.appInitializer.InitializeAppAsync(connectionString);
                IReadOnlyDictionary<Uri, RssFeedXmlParsingConfig> rssFeedXmlParsingConfigDictionary = 
                    await this.appInitializer.FinAllRssFeedXmlParsingConfigAsync(connectionString);
                ///
                ViewModelCompositionRoot viewModelCompositionRoot = new ViewModelCompositionRoot(connectionString, rssFeedXmlParsingConfigDictionary);
                MainWindowViewModel mainWindowViewModel = viewModelCompositionRoot.CreateMainWindowViewModel();
                this.ShowMainWindow(mainWindowViewModel);
            }
            catch (Exception exception)
            {
                this.ShowErrorInMainThread(exception);
            }
            finally
            {
                waitingWindow.Close();
            }
        }


        ////////////////////////////////////////////////////////////////////////////


        private void ShowErrorInMainThread(Exception exception)
        {
            this.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(exception.ToString());
            });
        }


        private string GetConnectionStringJsonFullFilePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Resource", "Config", "ConnectionString.json");
        }


        private WaitingWindow ShowWaitingWindow()
        {
            WaitingWindow waitingWindow = new WaitingWindow();
            waitingWindow.Show();
            return waitingWindow;
        }


        private void ShowMainWindow(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowView.MainWindow mainWindow = new MainWindowView.MainWindow();
            mainWindow.DataContext = mainWindowViewModel;
            mainWindow.Show();
        }

    }

}
