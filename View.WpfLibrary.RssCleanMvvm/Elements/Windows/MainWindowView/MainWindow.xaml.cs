using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            this.RegisterMessageHandler();
        }


        private void RegisterMessageHandler()
        {
            WeakReferenceMessenger.Default.Register<Exception, string>(this, SharedMessengerKey.Exception_ErrorKey, (_, exception) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(exception.ToString());
                });
            });
        }


        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
