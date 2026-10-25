using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.MainWindowViewModelModule.Class;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;

namespace CompositionRoot.WIndowsFormApp.ViewModules
{
    /// <summary>
    /// پنجره‌ی اصلی برنامه ، معادل MainWindow در نسخه‌ی WPF .
    /// منوی Settings (تغییر Theme و Language) در این نسخه حذف شده است .
    /// </summary>
    internal sealed class MainForm : Form
    {

        private readonly MainWindowViewModel viewModel;

        private readonly RssUserControl rssUserControl;

        private readonly MenuStrip menuStrip;

        private readonly ToolStripMenuItem newsFeedMenuItem;

        private readonly ToolStripMenuItem getNewNewsMenuItem;

        private readonly ToolStripMenuItem cancelCurrentOperationMenuItem;

        private readonly ToolStripMenuItem saveAllMenuItem;

        private readonly StatusStrip statusStrip;

        private readonly ToolStripStatusLabel statusLabel;


        ///////////////////////////////////////////////////////////////////////


        public MainForm(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;

            this.Text = "RssCleanMvvm - Windows Forms";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(900, 600);
            this.KeyPreview = true;

            /// ===== منوی اصلی =====
            /// در نسخه‌ی WPF علاوه بر منوی News Feed ، منوی Settings هم وجود داشت که فقط -
            /// شامل تغییر Theme و Language بود . چون این دو قابلیت در WinForms لازم نیستند ، -
            /// منوی Settings کاملا حذف شده است .
            this.saveAllMenuItem = new ToolStripMenuItem("ذخیره همه")
            {
                Enabled = false,
                ToolTipText = "ذخیره‌ی همه‌ی تغییرات در دیتابیس (در این نسخه پیاده‌سازی نشده است)"
            };

            this.getNewNewsMenuItem = new ToolStripMenuItem("دریافت خبر جدید")
            {
                ToolTipText = "دریافت خبرهای جدید از خبرگزاری انتخاب‌شده"
            };
            this.getNewNewsMenuItem.Click += async (_, _) => await this.DownloadRssItemsAsync();

            this.cancelCurrentOperationMenuItem = new ToolStripMenuItem("لغو عملیات جاری")
            {
                ToolTipText = "لغو عملیات دریافت خبر که در حال اجراست"
            };
            this.cancelCurrentOperationMenuItem.Click += (_, _) => this.CancelCurrentOperation();

            this.newsFeedMenuItem = new ToolStripMenuItem("اخبار");
            this.newsFeedMenuItem.DropDownItems.Add(this.getNewNewsMenuItem);
            this.newsFeedMenuItem.DropDownItems.Add(this.cancelCurrentOperationMenuItem);
            this.newsFeedMenuItem.DropDownItems.Add(new ToolStripSeparator());
            this.newsFeedMenuItem.DropDownItems.Add(this.saveAllMenuItem);

            /// هنگام باز شدن منو ، وضعیت فعال بودن آیتم‌ها را با انتخاب فعلی هم‌گام می‌کنیم .
            this.newsFeedMenuItem.DropDownOpening += (_, _) => this.UpdateMenuState();
            this.newsFeedMenuItem.DropDownOpened += (_, _) => this.UpdateMenuState();

            this.menuStrip = new MenuStrip
            {
                Dock = DockStyle.Top,
                RightToLeft = RightToLeft.Yes
            };
            this.menuStrip.Items.Add(this.newsFeedMenuItem);

            /// ===== نوار وضعیت =====
            this.statusLabel = new ToolStripStatusLabel
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight,
                Text = this.viewModel.StatusMessage ?? string.Empty
            };

            this.statusStrip = new StatusStrip
            {
                Dock = DockStyle.Bottom,
                RightToLeft = RightToLeft.Yes,
                SizingGrip = false
            };
            this.statusStrip.Items.Add(this.statusLabel);

            /// ===== محتوای اصلی =====
            this.rssUserControl = new RssUserControl(this.viewModel.RssUserControlViewModel)
            {
                Dock = DockStyle.Fill
            };
            this.rssUserControl.SelectedFeedChanged += (_, _) => this.UpdateMenuState();

            /// ترتیب اضافه کردن کنترل‌ها برای Dock کردن صحیح مهم است :
            /// ابتدا Fill ، بعد Bottom و در آخر Top .
            this.Controls.Add(this.rssUserControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;

            this.RegisterMessageHandler();
            this.viewModel.PropertyChanged += this.MainWindowViewModel_PropertyChanged;

            this.UpdateMenuState();
        }


        ///////////////////////////////////////////////////////////////////////


        private void RegisterMessageHandler()
        {
            /// خطاهایی که ViewModel ها با Messenger می‌فرستند ، اینجا نمایش داده می‌شوند .
            WeakReferenceMessenger.Default.Register<Exception, string>(this, SharedMessengerKey.Exception_ErrorKey, (_, exception) =>
            {
                this.ShowError(exception);
            });
        }


        private void MainWindowViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.StatusMessage))
            {
                UiThread.Post(this, () => this.statusLabel.Text = this.viewModel.StatusMessage ?? string.Empty);
            }
        }


        ///////////////////////////////////////////////////////////////////////


        private RssFeedViewModel? GetSelectedRssFeed()
        {
            return this.viewModel.RssUserControlViewModel.SelectedRssSiteViewModel?.SelectedRssFeedViewModel;
        }


        private void UpdateMenuState()
        {
            RssFeedViewModel? selectedRssFeed = this.GetSelectedRssFeed();

            this.getNewNewsMenuItem.Enabled = selectedRssFeed != null &&
                selectedRssFeed.DownLoadRssItemsCommand.CanExecute(null);

            this.cancelCurrentOperationMenuItem.Enabled = selectedRssFeed != null &&
                selectedRssFeed.CancelCurrentOperationCommand.CanExecute(null);

            /// دکمه‌ی «ذخیره همه» در نسخه‌ی WPF هم Command نداشت و فقط نمایشی بود .
            this.saveAllMenuItem.Enabled = false;
        }


        private async Task DownloadRssItemsAsync()
        {
            RssFeedViewModel? selectedRssFeed = this.GetSelectedRssFeed();
            if (selectedRssFeed == null)
                return;

            try
            {
                await selectedRssFeed.DownLoadRssItemsCommand.ExecuteAsync(null);
            }
            catch (Exception exception)
            {
                this.ShowError(exception);
            }
            finally
            {
                this.UpdateMenuState();
            }
        }


        private void CancelCurrentOperation()
        {
            RssFeedViewModel? selectedRssFeed = this.GetSelectedRssFeed();
            if (selectedRssFeed == null)
                return;

            if (selectedRssFeed.CancelCurrentOperationCommand.CanExecute(null) == true)
                selectedRssFeed.CancelCurrentOperationCommand.Execute(null);

            this.UpdateMenuState();
        }


        private void ShowError(Exception exception)
        {
            if (this.IsDisposed == true)
                return;

            UiThread.Post(this, () =>
            {
                MessageBox.Show(this, exception.ToString(), "خطا", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            });
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.Cancel == false)
            {
                try
                {
                    /// معادل EventTrigger مربوط به رویداد Closing در نسخه‌ی WPF .
                    this.viewModel.ExitCommand.Execute(null);
                }
                catch (Exception)
                {
                    /// بسته شدن پنجره نباید به خاطر خطای ExitCommand متوقف شود .
                }
            }

            base.OnFormClosing(e);
        }


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            this.viewModel.PropertyChanged -= this.MainWindowViewModel_PropertyChanged;

            base.OnFormClosed(e);
        }

    }
}
