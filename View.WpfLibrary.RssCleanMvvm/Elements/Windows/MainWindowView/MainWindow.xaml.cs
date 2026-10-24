using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
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

            // در MainWindow constructor (بعد از InitializeComponent، یا بعد از ساخت MenuItems):
            this.CancelDownloadMenuItem.SetCurrentValue(MenuItem.IsEnabledProperty, false);
            this.GetNewNewsMenuItem.SetCurrentValue(MenuItem.IsEnabledProperty, false);

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
