using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using ViewModel.Services.SharedMessengerKeyService.Class;

namespace View.WpfLibrary.RssCleanMvvm.Elements.Windows.MainWindowView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            this.InitializeView();
        }


        ///////////////////////////////////////////////////


        private void InitializeView()
        {
            this.RegisterErrorMessageHandler();
        }


        private void RegisterErrorMessageHandler()
        {
            WeakReferenceMessenger.Default.Register<Exception, string>(this, SharedMessengerKey.Exception_ErrorKey, (_, exception) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(exception.ToString());
                });
            });
        }

    }
}
