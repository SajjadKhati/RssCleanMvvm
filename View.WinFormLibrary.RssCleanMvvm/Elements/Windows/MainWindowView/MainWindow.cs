using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using View.WinFormLibrary.RssCleanMvvm.AppResource.LanguageResource.Class;
using View.WinFormLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;
using View.WinFormLibrary.RssCleanMvvm.UiLogicModules.BindingModules.Class;
using ViewModel.DataModels.MessengerKey.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.Windows.MainWindowView
{
    /// <summary>
    /// معادلِ <c>MainWindow.xaml</c> در نسخه‌ی WPF :
    ///   • منوی «خبر خوان» (دریافت اخبار جدید / لغو دانلود جاری / ذخیره همه)
    ///   • محتوای RssUserControl
    ///   • نوار وضعیت (StatusMessage)
    ///
    /// منوی «تنظیمات» (تغییر تم/زبان) طبق درخواست، وجود ندارد .
    ///
    /// همه‌ی ارتباط‌ها با ViewModel از طریق بایندینگ است و نه فراخوانی مستقیم :
    ///   • متنِ وضعیت : BindOneWay
    ///   • آیتم‌های منو : BindCommand (Enabled از CanExecute)
    ///   • RssUserControl : ViewModel خودش را از همین ViewModel می‌گیرد .
    ///
    /// تنها استثناء، بستن پنجره است (معادل MainWindow_Closing در WPF) : چون باید
    /// منتظر اتمامِ async کامندِ «ذخیره‌ی موارد ذخیره‌نشده» بمانیم و بستن را لغو کنیم،
    /// همان الگوی code-behind نسخه‌ی WPF تکرار شده است .
    /// </summary>
    public sealed class MainWindow : Form
    {
        private readonly BindingScope bindings;

        private readonly RssUserControl rssUserControl;

        private readonly MenuStrip menuStrip;

        private readonly ToolStripMenuItem newsFeedMenuItem;

        private readonly ToolStripMenuItem getNewNewsMenuItem;

        private readonly ToolStripMenuItem cancelCurrentOperationMenuItem;

        private readonly ToolStripMenuItem saveAllMenuItem;

        private readonly StatusStrip statusStrip;

        private readonly ToolStripStatusLabel statusMessageLabel;

        private MainWindowViewModel? viewModel;

        private BindingScope? feedCommandBindings;

        private bool isCommitting;

        private bool isCloseReady;

        private bool isMessageHandlerRegistered;


        ///////////////////////////////////////////////////////////////////////


        public MainWindow()
        {
            this.bindings = new BindingScope();

            this.Text = PersianLanguageResource.ApplicationTitle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1280, 720);
            this.MinimumSize = new Size(760, 480);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Font = UiTheme.BaseFont;
            this.BackColor = UiTheme.ContentBackground;
            this.DoubleBuffered = true;

            /// ===== منوی اصلی =====
            /// فقط منوی «خبر خوان» وجود دارد؛ منوی «تنظیمات» (تم/زبان) حذف شده است .
            this.getNewNewsMenuItem = new ToolStripMenuItem(PersianLanguageResource.GetNewNews)
            {
                ToolTipText = PersianLanguageResource.GetNewNewsToolTip
            };

            this.cancelCurrentOperationMenuItem = new ToolStripMenuItem(PersianLanguageResource.CancelCurrentDownload)
            {
                ToolTipText = PersianLanguageResource.CancelOperationToolTip
            };

            this.saveAllMenuItem = new ToolStripMenuItem(PersianLanguageResource.SaveAll)
            {
                ToolTipText = PersianLanguageResource.SaveAllToolTip
            };

            this.newsFeedMenuItem = new ToolStripMenuItem(PersianLanguageResource.NewsFeedMenu);
            this.newsFeedMenuItem.DropDownItems.Add(this.getNewNewsMenuItem);
            this.newsFeedMenuItem.DropDownItems.Add(this.cancelCurrentOperationMenuItem);
            this.newsFeedMenuItem.DropDownItems.Add(new ToolStripSeparator());
            this.newsFeedMenuItem.DropDownItems.Add(this.saveAllMenuItem);

            this.menuStrip = new MenuStrip
            {
                Dock = DockStyle.Top,
                RightToLeft = RightToLeft.Yes,
                BackColor = UiTheme.Surface,
                ForeColor = UiTheme.TextPrimary,
                Font = UiTheme.BaseFont,
                ShowItemToolTips = true,
                Padding = new Padding(8, 2, 0, 2)
            };
            this.menuStrip.Items.Add(this.newsFeedMenuItem);

            /// ===== نوار وضعیت =====
            this.statusMessageLabel = new ToolStripStatusLabel
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight,
                Font = UiTheme.StatusFont,
                ForeColor = UiTheme.StatusText,
                Text = string.Empty
            };

            this.statusStrip = new StatusStrip
            {
                Dock = DockStyle.Bottom,
                RightToLeft = RightToLeft.Yes,
                SizingGrip = false,
                BackColor = UiTheme.Surface,
                Padding = new Padding(10, 0, 10, 0)
            };
            this.statusStrip.Items.Add(this.statusMessageLabel);

            /// ===== محتوای اصلی =====
            this.rssUserControl = new RssUserControl
            {
                Dock = DockStyle.Fill
            };
            this.rssUserControl.SelectedFeedChanged += this.RssUserControl_SelectedFeedChanged;

            /// ترتیب اضافه کردن برای Dock صحیح مهم است :
            /// ابتدا Fill ، بعد Bottom و در آخر Top .
            this.Controls.Add(this.rssUserControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);

            this.MainMenuStrip = this.menuStrip;

            this.UpdateFeedMenuCommands();
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// اتصالِ View به ViewModel (معادلِ <c>DataContext</c> در WPF) .
        /// پنجره‌ی اصلی را ViewDisplayer قبل از Show صدا می‌زند .
        /// </summary>
        public void SetViewModel(MainWindowViewModel mainWindowViewModel)
        {
            if (mainWindowViewModel == null)
                throw new ArgumentNullException(nameof(mainWindowViewModel));

            if (this.viewModel != null)
                throw new InvalidOperationException("ViewModel قبلاً تنظیم شده است .");

            this.viewModel = mainWindowViewModel;

            /// ===== متنِ نوار وضعیت =====
            this.bindings.BindOneWay(this, this.viewModel, nameof(MainWindowViewModel.StatusMessage),
                () => this.statusMessageLabel.Text = this.viewModel.StatusMessage ?? string.Empty);

            /// ===== کامندِ «ذخیره همه» (به RssUserControlViewModel مربوط است) =====
            this.bindings.BindCommand(this.saveAllMenuItem,
                this.viewModel.RssUserControlViewModel.CommitAllRssItemsCommand);

            /// ===== RssUserControl =====
            this.rssUserControl.SetViewModel(this.viewModel.RssUserControlViewModel);

            this.RegisterMessageHandler();
            this.UpdateFeedMenuCommands();
        }


        ///////////////////////////////////////////////////////////////////////


        private void RegisterMessageHandler()
        {
            if (this.isMessageHandlerRegistered == true)
                return;

            this.isMessageHandlerRegistered = true;

            /// خطاهایی که ViewModel ها با Messenger می‌فرستند، اینجا نمایش داده می‌شوند .
            WeakReferenceMessenger.Default.Register<Exception, string>(this, MessengerKey.Exception_ErrorKey,
                (object recipient, Exception exception) => this.ShowError(exception));
        }


        private void ShowError(Exception exception)
        {
            UiThread.Post(this, () =>
            {
                if (this.IsDisposed == true)
                    return;

                MessageBox.Show(this, exception.ToString(), PersianLanguageResource.ErrorCaption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            });
        }


        ///////////////////////////////////////////////////////////////////////


        private void RssUserControl_SelectedFeedChanged(object? sender, EventArgs e)
        {
            this.UpdateFeedMenuCommands();
        }


        /// <summary>
        /// آیتم‌های منو، به Command های فیدِ انتخاب‌شده بایند می‌شوند .
        ///
        /// در WPF این کار با یک Binding به مسیر
        /// <c>SelectedRssSiteViewModel.SelectedRssFeedViewModel.DownLoadRssItemsCommand</c> انجام می‌شد
        /// و WPF با تغییر مسیر، خودش Command را دوباره حل می‌کرد . در WinForms، چنین
        /// Bindingِ مسیردار و CommandManager ای وجود ندارد؛ بنابراین با تغییر انتخاب،
        /// همان دو Command را دوباره بایند می‌کنیم . این «اتصال مستقیم» نیست :
        /// همچنان از BindCommand استفاده می‌شود و View فقط Command را اجرا می‌کند .
        /// </summary>
        private void UpdateFeedMenuCommands()
        {
            this.feedCommandBindings?.Dispose();
            this.feedCommandBindings = new BindingScope();

            RssFeedViewModel? selectedFeed = this.viewModel?.RssUserControlViewModel
                .SelectedRssSiteViewModel?.SelectedRssFeedViewModel;

            this.feedCommandBindings.BindCommand(this.getNewNewsMenuItem, selectedFeed?.DownLoadRssItemsCommand);
            this.feedCommandBindings.BindCommand(this.cancelCurrentOperationMenuItem, selectedFeed?.CancelCurrentOperationCommand);
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// معادلِ <c>MainWindow_Closing</c> در نسخه‌ی WPF .
        ///
        /// چرا code-behind و نه Command؟
        ///   ۱) رویداد Closing به <see cref="FormClosingEventArgs"/> نیاز دارد تا بتوان بستن را لغو کرد .
        ///   ۲) Command Binding فقط Command را اجرا می‌کند و منتظر async نمی‌ماند؛
        ///      در نتیجه پنجره قبل از اتمامِ commit بسته می‌شد و کار نیمه‌کاره می‌ماند .
        ///   ۳) پس با await منتظر می‌مانیم و بعد خودمان Close() را صدا می‌زنیم .
        /// </summary>
        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            /// اگر commit قبلاً تمام شده و Close() را خودمان صدا زده‌ایم، جلوی بستن را نگیر .
            if (this.isCloseReady == true)
                return;

            /// اگر commit در حال اجراست، درخواست بستنِ جدیدِ کاربر را رد می‌کنیم (وگرنه double-execution).
            if (this.isCommitting == true)
            {
                e.Cancel = true;
                return;
            }

            /// در خاموش‌شدن سیستم/بستن اجباری، وقتِ commit کردن نداریم و نباید بستن را لغو کنیم .
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
                return;

            IAsyncRelayCommand? commitAllRssItemsNoResultCommand = this.viewModel?.RssUserControlViewModel
                .CommitAllRssItemsNoResultCommand;

            if (commitAllRssItemsNoResultCommand == null ||
                commitAllRssItemsNoResultCommand.CanExecute(null) == false)
                return;

            /// از این‌جا به بعد واقعاً باید commit انجام شود؛ ابتدا بستن را لغو می‌کنیم .
            e.Cancel = true;
            this.isCommitting = true;

            await this.CommitAndCloseAsync(commitAllRssItemsNoResultCommand);
        }


        /// <summary>
        /// کامندِ commit را اجرا و پس از اتمام (موفق یا خطا) پنجره را می‌بندد .
        /// </summary>
        private async Task CommitAndCloseAsync(IAsyncRelayCommand commitAllRssItemsNoResultCommand)
        {
            try
            {
                /// خودِ کامند، confirmation و منطق را مدیریت می‌کند .
                await commitAllRssItemsNoResultCommand.ExecuteAsync(null);
            }
            finally
            {
                this.isCommitting = false;
            }

            /// حالا commit تمام شده؛ پرچمِ «آماده‌ی بستن» را روشن می‌کنیم .
            this.isCloseReady = true;

            /// ⚠️ چرا BeginInvoke؟
            /// اگر کل جریان sync تمام شود (مثلاً کاربر «نه» زده باشد)، Close() هنوز داخل
            /// همان call stack ئه OnFormClosing اجرا می‌شود؛ ولی WinForms هنوز e.Cancel=true
            /// را پردازش نکرده است . با BeginInvoke، بستن به صف پیام‌ها می‌رود و بعد از
            /// برگشت از handler اجرا می‌شود (معادل Task.Yield/Dispatcher.BeginInvoke در WPF).
            if (this.IsDisposed == false)
                this.BeginInvoke(new Action(this.Close));
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                if (this.isMessageHandlerRegistered == true)
                {
                    WeakReferenceMessenger.Default.UnregisterAll(this);
                    this.isMessageHandlerRegistered = false;
                }

                this.rssUserControl.SelectedFeedChanged -= this.RssUserControl_SelectedFeedChanged;

                this.feedCommandBindings?.Dispose();
                this.feedCommandBindings = null;

                this.bindings.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
