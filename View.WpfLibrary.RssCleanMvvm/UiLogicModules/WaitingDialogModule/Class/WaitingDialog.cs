using System.Windows;
using View.WpfLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView;
using ViewModel.Contracts.ViewContracts.WaitingDialog.Interface;

namespace View.WpfLibrary.RssCleanMvvm.UiLogicModules.WaitingDialogModule.Class
{
    public class WaitingDialog : IWaitingDialog
    {

        private Window ownerWindow;


        /////////////////////////////////////////////////////////////////////////////


        public WaitingDialog(Window ownerWindow)
        {
            this.ownerWindow = ownerWindow;
        }


        /////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// منطق را در حین نمایش پنجره‌ی انتظار اجرا می‌کند.
        ///
        /// ⚠️ نکته‌ی کلیدی درباره‌ی جریان اجرا:
        /// این متد، logicFuncAsync را مستقیماً صدا نمی‌زند. فقط آن را به عنوان
        /// delegate به WaitingWindow.Loaded می‌سپارد. سپس ShowDialog() را صدا
        /// می‌زند (که UI thread را بلاک می‌کند، ولی nested pump دارد).
        ///
        /// جریان واقعی:
        ///   RunLogicInWaitingDialog (این متد)
        ///     ├── WaitingWindow ساخته می‌شود
        ///     ├── Loaded += handler (فقط ثبت، نه اجرا)
        ///     ├── ShowDialog() → پنجره نمایش داده می‌شود، nested pump شروع
        ///     │     └── WPF، Loaded event را fire می‌کند
        ///     │           └── handler اجرا → await logicFuncAsync()
        ///     │                 └── اجرای واقعی منطق
        ///     ├── (nested pump منتظر: پیام‌های UI + continuationها)
        ///     └── وقتی handler تمام شد → finally → waitingWindow.Close()
        ///           → ShowDialog برمی‌گردد
        ///           → RunLogicInWaitingDialog برمی‌گردد
        ///
        /// ⚠️ پس اگر RunLogicInWaitingDialog در جایی throw بدهد (مثلاً در ساخت
        /// WaitingWindow)، logicFuncAsync هرگز اجرا نمی‌شود — چون handler هنوز
        /// ثبت نشده. این، تفاوت ظریف «قرارداد» و «اجرای واقعی» است.
        /// </summary>
        /// <param name="logicFuncAsync"></param>
        public void RunLogicInWaitingDialog(Func<Task> logicFuncAsync)
        {
            WaitingWindow waitingWindow = new WaitingWindow();
            waitingWindow.Owner = this.ownerWindow;

            /// ثبت handler برای Loaded. این فقط «تعریف» است، نه اجرا.
            /// اجرای واقعی، وقتی رخ می‌دهد که WPF پس از نمایش WaitingWindow،
            /// Loaded event را fire کند.
            waitingWindow.Loaded += async (object sender, RoutedEventArgs e) =>
            {
                try
                {
                    /// از اینجا، منطق تجاری واقعاً شروع می‌شود.
                    await logicFuncAsync();
                }
                finally
                {
                    /// در هر حالت (موفق، خطا، لغو)، پنجره بسته می‌شود
                    /// تا ShowDialog برنگردد و caller آزاد شود.
                    waitingWindow.Close();
                }
            };
            /// ShowDialog، تا بسته‌شدن پنجره، به caller برنمی‌گردد.
            /// ولی nested pump داخلش، پیام‌های UI و continuationهای async را
            /// پردازش می‌کند — پس Loaded fire می‌شود، await اجرا می‌شود، و Close
            /// نهایتاً ShowDialog را برمی‌گرداند.
            waitingWindow.ShowDialog();
        }

    }
}
