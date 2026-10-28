using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Windows;
using ViewModel.DataModels.MessengerKey.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace View.WpfLibrary.RssCleanMvvm.Elements.Windows.MainWindowView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// چرا از code-behind به‌جای XAML (EventTrigger + InvokeCommandAction) استفاده شده؟
        ///
        /// ۱. رویداد Closing به CancelEventArgs نیاز دارد تا بتوان بستن پنجره را لغو کرد
        ///    (وقتی commit در حال اجراست یا کاربر "انصراف" زده). InvokeCommandAction
        ///    فقط Command را اجرا می‌کند و به CancelEventArgs دسترسی ندارد.
        ///
        /// ۲. InvokeCommandAction برای IAsyncRelayCommand، Execute را صدا می‌زند (نه
        ///    ExecuteAsync) و منتظر async نمی‌ماند. نتیجه: بعد از return از handler،
        ///    WPF پنجره را می‌بندد و Application.Shutdown شروع می‌شود؛ در حالی که
        ///    commit هنوز در حال اجراست و نیمه‌کاره کشته می‌شود.
        ///
        /// ۳. پس تنها راه اطمینان از "منتظر ماندن تا اتمام commit، سپس بستن"،
        ///    code-behind با await است.
        /// </summary>
        private bool isCommitting = false;

        /// <summary>
        /// پرچم "آماده‌ی بستن". وقتی true شد، یعنی commit تمام شده و این Close()
        /// نهایی را خودمان صدا زده‌ایم (نه کاربر) — پس نباید دوباره commit شود.
        /// این پرچم از loop بی‌نهایت (Close → Closing → Close → ...) جلوگیری می‌کند.
        /// </summary>
        private bool isCloseReady = false;


        /////////////////////////////////////////////////////////////////////////////


        public MainWindow()
        {
            InitializeComponent();

            this.InitializeView();
        }


        /////////////////////////////////////////////////////////////////////////////


        private void InitializeView()
        {
            this.RegisterMessageHandler();
        }


        /// <summary>
        /// ⚠️ چرا از code-behind به‌جای XAML (EventTrigger + InvokeCommandAction) استفاده شده؟
        ///
        /// ۱. رویداد Closing به CancelEventArgs نیاز دارد تا بتوان بستن پنجره را لغو کرد
        ///    (وقتی commit در حال اجراست یا کاربر "انصراف" زده). InvokeCommandAction
        ///    فقط Command را اجرا می‌کند و به CancelEventArgs دسترسی ندارد.
        ///
        /// ۲. InvokeCommandAction برای IAsyncRelayCommand، Execute را صدا می‌زند (نه
        ///    ExecuteAsync) و منتظر async نمی‌ماند. نتیجه: بعد از return از handler،
        ///    WPF پنجره را می‌بندد و Application.Shutdown شروع می‌شود؛ در حالی که
        ///    commit هنوز در حال اجراست و نیمه‌کاره کشته می‌شود.
        ///
        /// ۳. پس تنها راه اطمینان از "منتظر ماندن تا اتمام commit، سپس بستن"،
        ///    code-behind با await است.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            /// اگر commit قبلاً تمام شده و این Close() را خودمان صدا زده‌ایم،
            /// e.Cancel را ست نمی‌کنیم (پیش‌فرض false) تا پنجره واقعاً بسته شود.
            if (this.isCloseReady == true)
                return;

            /// اگر commit در حال اجراست، درخواست Close جدید کاربر را رد می‌کنیم.
            /// e.Cancel=true مانع بسته شدن پنجره در طول commit می‌شود.
            if (this.isCommitting == true)
            {
                e.Cancel = true;
                return;
            }

            /// دسترسی به ViewModel مربوط به منطق RSS (که کامند commit را دارد).
            RssUserControlViewModel rssUserControlViewModel = (this.DataContext as MainWindowViewModel)?.RssUserControlViewModel;

            /// اگر ViewModel ساخته نشده (مثلاً خطا در startup)، بگذار پنجره بسته شود
            /// (e.Cancel=false پیش‌فرض).
            if (rssUserControlViewModel is null)
                return;

            /// اگر آیتم ذخیره‌نشده‌ای وجود ندارد (CanExecute=false)،
            /// نیازی به commit نیست؛ بگذار پنجره بسته شود.
            if (rssUserControlViewModel.CommitAllRssItemsNoResultCommand.CanExecute(null) == false)
                return;

            /// از این‌جا به بعد، واقعاً باید commit انجام شود.
            /// ابتدا بستن پنجره را لغو می‌کنیم (تا Application.Shutdown شروع نشود
            /// و commit نیمه‌کاره کشته نشود).
            e.Cancel = true;

            /// پرچم "در حال commit" را روشن می‌کنیم تا Closeهای جدید کاربر در این
            /// بازه، رد شوند (و از double-execution جلوگیری شود).
            this.isCommitting = true;

            await this.CommitAndCloseAsync(rssUserControlViewModel);
        }


        /////////////////////////////////////////////////////////////////////////////


        private void RegisterMessageHandler()
        {
            WeakReferenceMessenger.Default.Register<Exception, string>(this, MessengerKey.Exception_ErrorKey,
                (_, exception) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(exception.ToString());
                    });
                });
        }


        /// <summary>
        /// commit را اجرا می‌کند و پس از اتمام (موفق یا خطا)، پنجره را می‌بندد.
        /// </summary>
        /// <param name="rssUserControlViewModel"></param>
        /// <returns></returns>
        private async Task CommitAndCloseAsync(RssUserControlViewModel rssUserControlViewModel)
        {
            try
            {
                /// اجرای کامند commit (که خودش، confirmation و منطق را مدیریت می‌کند).
                /// اینجا واقعاً await می‌کنیم (برخلاف InvokeCommandAction).
                await rssUserControlViewModel.CommitAllRssItemsNoResultCommand.ExecuteAsync(null);
            }
            finally
            {
                /// در هر حالت (موفق، خطا، لغو)، پرچم "در حال commit" را خاموش می‌کنیم
                /// تا وضعیت سازگار بماند.
                this.isCommitting = false;
            }

            /// حالا commit تمام شده؛ پرچم "آماده‌ی بستن" را روشن می‌کنیم
            /// تا Closing بعدی (که خودمان trigger می‌کنیم) بلوکه نشود.
            this.isCloseReady = true;

            /// ⚠️ چرا Task.Yield؟
            /// اگه کل جریان sync تموم بشه (مثلاً کاربر «نه» زده و commit انجام نشد)،
            /// Close() هنوز داخل call stack ئه Closing handler اجرا می‌شه — و WPF
            /// هنوز e.Cancel=true رو پردازش نکرده → پنجره در closing state →
            /// Close() استثنا: "Cannot call Close() while a Window is closing."
            ///
            /// Task.Yield، continuation رو به dispatcher queue می‌فرسته → Closing
            /// handler برمی‌گرده، WPF e.Cancel رو پردازش می‌کنه، بعد continuation
            /// اجرا می‌شه (خارج از closing state) → Close() امن.
            await Task.Yield();

            /// چرا Close()؟
            /// چون در ابتدای این Closing، e.Cancel=true ست شد و پنجره بسته نشد.
            /// تنها راه بستن پنجره، trigger کردن یک Closing جدید است — که با Close()
            /// انجام می‌شود. در آن Closing جدید، isCloseReady=true → return فوری
            /// → e.Cancel=false (پیش‌فرض) → پنجره بسته می‌شود.
            this.Close();
        }

    }
}
