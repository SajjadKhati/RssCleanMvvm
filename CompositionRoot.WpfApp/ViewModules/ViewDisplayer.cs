using System.Windows;
using System.Windows.Threading;
using View.WpfLibrary.RssCleanMvvm.Elements.Windows.MainWindowView;
using View.WpfLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;

namespace CompositionRoot.WpfApp.ViewModules
{
    public class ViewDisplayer
    {

        private Dispatcher mainThreadDispatcher;


        ///////////////////////////////////////////////////////////


        public ViewDisplayer(Dispatcher mainThreadDispatcher)
        {
            this.mainThreadDispatcher = mainThreadDispatcher;
        }


        ///////////////////////////////////////////////////////////


        public void ShowErrorInMainThread(Exception exception, string messageCaption)
        {
            this.mainThreadDispatcher.Invoke(() =>
            {
                MessageBox.Show(exception.ToString(), messageCaption);
            });
        }


        public WaitingWindow ShowWaitingWindow()
        {
            WaitingWindow waitingWindow = new WaitingWindow();
            waitingWindow.Show();
            return waitingWindow;
        }


        public void ShowMainWindowAndSetupApp(MainWindowViewModel mainWindowViewModel, System.Windows.Application currentApp)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = mainWindowViewModel;
            currentApp.MainWindow = mainWindow;
            mainWindow.Show();
        }

    }
}
