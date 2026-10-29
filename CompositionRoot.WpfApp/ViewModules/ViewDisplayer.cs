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
            WaitingWindow waitingWindow = new WaitingWindow("در حال بارگذاری ...");
            waitingWindow.Show();
            return waitingWindow;
        }


        public void ShowMainWindowAndSetup(MainWindow mainWindow, MainWindowViewModel mainWindowViewModel, 
            System.Windows.Application currentApp)
        {
            mainWindow.DataContext = mainWindowViewModel;
            currentApp.MainWindow = mainWindow;
            mainWindow.Show();
        }

    }
}
