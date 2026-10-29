using View.WinFormLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView;
using ViewModel.Contracts.ViewContracts.WaitingDialog.Interface;

namespace View.WinFormLibrary.RssCleanMvvm.UiLogicModules.WaitingDialogModule.Class
{
    /// <summary>
    /// معادلِ <c>WaitingDialog</c> در نسخه‌ی WPF .
    ///
    /// قرارداد (همان قرارداد WPF):
    ///   • پنجره‌ی انتظار modal نمایش داده می‌شود .
    ///   • منطقِ async، بعد از نمایشِ پنجره اجرا می‌شود (نه قبل از آن) .
    ///   • پس از اتمام (یا خطای) منطق، پنجره بسته می‌شود .
    ///   • از دید caller، این متد synchronous است (تا بسته شدن پنجره برنمی‌گردد) .
    ///
    /// چرا منطق در رویداد Shown اجرا می‌شود و نه مستقیماً؟
    /// چون ShowDialog تا بسته شدن پنجره برنمی‌گردد، ولی پیام‌لوپِ تو در توی خودش را
    /// اجرا می‌کند؛ پس async/await داخل Shown ادامه پیدا می‌کند و UI فریز نمی‌شود .
    /// این همان الگوی WaitingWindow.Loaded در WPF است .
    /// </summary>
    public sealed class WaitingDialog : IWaitingDialog
    {
        private readonly Form ownerWindow;


        ///////////////////////////////////////////////////////////////////////


        public WaitingDialog(Form ownerWindow)
        {
            this.ownerWindow = ownerWindow;
        }


        ///////////////////////////////////////////////////////////////////////


        public void RunLogicInWaitingDialog(Func<Task> logicFuncAsync, string waitingText)
        {
            if (logicFuncAsync == null)
                throw new ArgumentNullException(nameof(logicFuncAsync));

            WaitingWindow waitingWindow = new(waitingText);

            waitingWindow.Shown += async (object? sender, EventArgs eventArgs) =>
            {
                try
                {
                    /// از اینجا منطقِ تجاری واقعاً شروع می‌شود .
                    await logicFuncAsync();
                }
                finally
                {
                    /// در هر حالت (موفق، خطا، لغو) پنجره بسته می‌شود تا ShowDialog برنگردد .
                    if (waitingWindow.IsDisposed == false)
                        waitingWindow.CloseFromOwner();
                }
            };

            if (this.ownerWindow.IsDisposed == false && this.ownerWindow.IsHandleCreated == true)
            {
                waitingWindow.StartPosition = FormStartPosition.CenterParent;
                waitingWindow.ShowDialog(this.ownerWindow);
            }
            else
            {
                waitingWindow.StartPosition = FormStartPosition.CenterScreen;
                waitingWindow.ShowDialog();
            }
        }

    }
}
