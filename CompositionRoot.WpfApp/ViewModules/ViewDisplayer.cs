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


        public void ShowErrorInMainThread(Exception exception)
        {
            this.mainThreadDispatcher.Invoke(() =>
            {
                MessageBox.Show(exception.ToString());
            });
        }


        public WaitingWindow ShowWaitingWindow()
        {
            WaitingWindow waitingWindow = new WaitingWindow();
            waitingWindow.Show();
            return waitingWindow;
        }


        public void ShowMainWindow(MainWindowViewModel mainWindowViewModel)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = mainWindowViewModel;
            mainWindow.Show();
        }

    }
}
