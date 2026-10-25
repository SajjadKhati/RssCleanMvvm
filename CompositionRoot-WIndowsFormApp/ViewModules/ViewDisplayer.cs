using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;

namespace CompositionRoot.WIndowsFormApp.ViewModules
{
    /// <summary>
    /// همان نقش ViewDisplayer در نسخه‌ی WPF را دارد :
    /// نمایش خطا روی نخ UI و ساخت و نمایش پنجره‌ی اصلی .
    /// </summary>
    internal sealed class ViewDisplayer
    {

        /// <summary>
        /// کنترلی که برای Invoke کردن روی نخ UI استفاده می‌شود .
        /// ابتدا پنجره‌ی انتظار و بعد از ساخته شدن ، پنجره‌ی اصلی جای آن را می‌گیرد .
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
                Form? owner = this.uiAnchor as Form;

                if (owner != null && owner.IsDisposed == false && owner.IsHandleCreated == true)
                {
                    MessageBox.Show(owner, exception.ToString(), messageCaption, MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
                else
                {
                    MessageBox.Show(exception.ToString(), messageCaption, MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            }

            Control? anchor = this.uiAnchor;
            if (anchor == null || anchor.IsDisposed == true || anchor.IsHandleCreated == false)
            {
                ShowError();
                return;
            }

            if (anchor.InvokeRequired == true)
                anchor.BeginInvoke(new Action(ShowError));
            else
                ShowError();
        }


        /// <summary>
        /// پنجره‌ی اصلی را می‌سازد و آن را به عنوان MainForm شی ApplicationContext تنظیم می‌کند .
        /// به این ترتیب با بسته شدن پنجره‌ی اصلی ، پیام‌لوپ برنامه خاتمه پیدا می‌کند .
        /// </summary>
        public MainForm ShowMainWindowAndSetupApp(MainWindowViewModel mainWindowViewModel, ApplicationContext applicationContext)
        {
            MainForm mainForm = new MainForm(mainWindowViewModel);
            this.AttachUiAnchor(mainForm);

            applicationContext.MainForm = mainForm;
            mainForm.Show();

            return mainForm;
        }

    }
}
