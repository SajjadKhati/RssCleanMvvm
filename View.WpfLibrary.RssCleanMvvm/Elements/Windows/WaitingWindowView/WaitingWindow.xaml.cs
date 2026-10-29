using System.Windows;

namespace View.WpfLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView
{
    /// <summary>
    /// Interaction logic for WaitingWindow.xaml
    /// </summary>
    public partial class WaitingWindow : Window
    {

        public WaitingWindow()
        {
            InitializeComponent();
        }


        public WaitingWindow(string waitingText) : this()
        {
            this.WaitingTextBlock.Text = waitingText;
        }

    }
}
