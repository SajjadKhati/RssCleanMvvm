using System.Windows;
using ViewModel.Contracts.ViewContracts.ConfirmationDialog.Interface;

namespace View.WpfLibrary.RssCleanMvvm.UiLogicModules.ConfirmationDialog.Class
{
    public class ConfirmationDialog : IConfirmationDialog
    {

        public bool ShowConfirmation(string message, string title)
        {
            MessageBoxResult messageBoxResult = System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                return MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            });

            return messageBoxResult == MessageBoxResult.Yes;
        }

    }
}
