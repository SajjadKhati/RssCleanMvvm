using ViewModel.Contracts.ViewContracts.ConfirmationDialog.Interface;

namespace View.WinFormLibrary.RssCleanMvvm.UiLogicModules.ConfirmationDialog.Class
{
    /// <summary>
    /// معادلِ <c>ConfirmationDialog</c> در نسخه‌ی WPF .
    ///
    /// ViewModel فقط <see cref="IConfirmationDialog"/> را می‌شناسد و نمی‌داند پشت آن
    /// MessageBox است یا هر چیز دیگر . این جداسازی، همان الگوی WPF است .
    /// </summary>
    public sealed class ConfirmationDialog : IConfirmationDialog
    {
        public bool ShowConfirmation(string message, string title)
        {
            Form? ownerForm = Form.ActiveForm;

            DialogResult result;

            if (ownerForm != null && ownerForm.IsDisposed == false)
            {
                result = MessageBox.Show(ownerForm, message, title, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            }
            else
            {
                result = MessageBox.Show(message, title, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            }

            return result == DialogResult.Yes;
        }
    }
}
