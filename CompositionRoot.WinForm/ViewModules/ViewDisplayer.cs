using View.WinFormLibrary.RssCleanMvvm.AppResource.LanguageResource.Class;
using View.WinFormLibrary.RssCleanMvvm.Elements.Windows.MainWindowView;
using View.WinFormLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;

namespace CompositionRoot.WinForm.ViewModules
{
    /// <summary>
    /// معادلِ <c>ViewDisplayer</c> در نسخه‌ی WPF :
    ///   • نمایش خطا روی نخ UI
    ///   • ساخت و نمایش پنجره‌ی انتظار
    ///   • ساخت و نمایش پنجره‌ی اصلی و ثبت آن به‌عنوان MainForm شی ApplicationContext
    /// </summary>
    internal sealed class ViewDisplayer
    {
        /// <summary>
        /// کنترلی که برای Invoke کردن روی نخ UI استفاده می‌شود .
        /// ابتدا پنجره‌ی انتظار و پس از ساخته شدن، پنجره‌ی اصلی جای آن را می‌گیرد .
        /// (معادلِ Dispatcher در نسخه‌ی WPF)
        /// </summary>
        private Control? uiAnchor;


        ///////////////////////////////////////////////////////////////////////


        public void AttachUiAnchor(Control control)
        {
            this.uiAnchor = control;
        }


        public void ShowErrorInMainThread(Exception exception, string messageCaption)
        {
            void ShowError()
            {
                if (this.uiAnchor is Form ownerForm &&
                    ownerForm.IsDisposed == false &&
                    ownerForm.IsHandleCreated == true)
                {
                    MessageBox.Show(ownerForm, exception.ToString(), messageCaption, MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    return;
                }

                MessageBox.Show(exception.ToString(), messageCaption, MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            }

            Control? anchor = this.uiAnchor;

            if (anchor == null || anchor.IsDisposed == true || anchor.IsHandleCreated == false)
            {
                ShowError();
                return;
            }

            /// اگر روی نخ UI هستیم، مستقیم نمایش می‌دهیم تا پیام قبل از ادامه‌ی جریان دیده شود .
            if (anchor.InvokeRequired == false)
            {
                ShowError();
                return;
            }

            try
            {
                anchor.BeginInvoke(new Action(ShowError));
            }
            catch (InvalidOperationException)
            {
                /// اگر هندلِ کنترل در این فاصله نابود شده باشد، پیام را بدون Owner نشان می‌دهیم .
                ShowError();
            }
        }


        /// <summary>
        /// پنجره‌ی انتظار را می‌سازد و (به‌صورت غیر modal) نمایش می‌دهد .
        /// معادلِ ShowWaitingWindow در WPF .
        /// </summary>
        public WaitingWindow ShowWaitingWindow()
        {
            WaitingWindow waitingWindow = new(PersianLanguageResource.Loading);

            this.AttachUiAnchor(waitingWindow);
            waitingWindow.Show();

            return waitingWindow;
        }


        /// <summary>
        /// پنجره‌ی اصلی را به ViewModel بایند می‌کند و به‌عنوان MainForm شی ApplicationContext
        /// تنظیم می‌کند تا با بسته شدن آن، پیام‌لوپ برنامه خاتمه یابد .
        /// معادلِ ShowMainWindowAndSetup در WPF .
        /// </summary>
        public void ShowMainWindowAndSetup(MainWindow mainWindow, MainWindowViewModel mainWindowViewModel,
            ApplicationContext currentApplicationContext)
        {
            mainWindow.SetViewModel(mainWindowViewModel);

            this.AttachUiAnchor(mainWindow);

            currentApplicationContext.MainForm = mainWindow;
            mainWindow.Show();
        }

    }
}
