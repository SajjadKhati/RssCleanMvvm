using System.Collections.Specialized;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;
using ViewModel.ViewModelModules.RssSiteViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace CompositionRoot.WIndowsFormApp.ViewModules
{
    /// <summary>
    /// معادل RssUserControl در نسخه‌ی WPF .
    /// انتخاب خبرگزاری و دسته‌بندی و نمایش لیست خبرهای دسته‌بندی انتخاب‌شده .
    /// </summary>
    internal sealed class RssUserControl : UserControl
    {

        public event EventHandler? SelectedFeedChanged;


        ///////////////////////////////////////////////////////////////////////


        private readonly RssUserControlViewModel viewModel;

        private readonly ComboBox siteComboBox;

        private readonly ComboBox feedComboBox;

        private readonly FlowLayoutPanel itemsPanel;

        private RssSiteViewModel? observedSite;

        private RssFeedViewModel? observedFeed;

        private bool isUpdatingSelection;


        ///////////////////////////////////////////////////////////////////////


        public RssUserControl(RssUserControlViewModel viewModel)
        {
            this.viewModel = viewModel;

            this.RightToLeft = RightToLeft.Yes;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            this.siteComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = nameof(RssSiteViewModel.SiteName),
                RightToLeft = RightToLeft.Yes,
                Margin = new Padding(6)
            };

            this.feedComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = nameof(RssFeedViewModel.Title),
                RightToLeft = RightToLeft.Yes,
                Margin = new Padding(6)
            };

            this.itemsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(10),
                RightToLeft = RightToLeft.Yes
            };

            this.InitializeLayout();

            this.siteComboBox.SelectedIndexChanged += this.SiteComboBox_SelectedIndexChanged;
            this.feedComboBox.SelectedIndexChanged += this.FeedComboBox_SelectedIndexChanged;
            this.itemsPanel.SizeChanged += this.ItemsPanel_SizeChanged;

            this.viewModel.RssSiteViewModels.CollectionChanged += this.RssSiteViewModels_CollectionChanged;
        }


        ///////////////////////////////////////////////////////////////////////


        private void InitializeLayout()
        {
            TableLayoutPanel headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10, 6, 10, 6),
                BackColor = Color.White
            };
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label siteLabel = new Label
            {
                Text = "سایت خبری :",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                BackColor = Color.Transparent,
                Margin = new Padding(6, 10, 6, 6)
            };

            Label feedLabel = new Label
            {
                Text = "دسته‌بندی خبر :",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                BackColor = Color.Transparent,
                Margin = new Padding(6, 10, 6, 6)
            };

            headerLayout.Controls.Add(siteLabel, 0, 0);
            headerLayout.Controls.Add(this.siteComboBox, 1, 0);
            headerLayout.Controls.Add(feedLabel, 0, 1);
            headerLayout.Controls.Add(this.feedComboBox, 1, 1);

            TableLayoutPanel rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.White
            };
            rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(headerLayout, 0, 0);
            rootLayout.Controls.Add(this.itemsPanel, 0, 1);

            this.Controls.Add(rootLayout);
        }


        ///////////////////////////////////////////////////////////////////////


        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            await this.LoadRssSitesAsync();
        }


        private async Task LoadRssSitesAsync()
        {
            try
            {
                /// معادل EventTrigger مربوط به رویداد Loaded در نسخه‌ی WPF .
                await this.viewModel.LoadRssSiteCommand.ExecuteAsync(null);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.ToString(), "خطا در بارگذاری سایت‌های خبری",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            }
        }


        ///////////////////////////////////////////////////////////////////////


        private void RssSiteViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UiThread.Post(this, this.RefreshSiteComboBox);
        }


        private void RssFeedViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UiThread.Post(this, this.RefreshFeedComboBox);
        }


        private void RssItemViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UiThread.Post(this, this.RebuildItemViews);
        }


        ///////////////////////////////////////////////////////////////////////


        private void RefreshSiteComboBox()
        {
            if (this.IsDisposed == true)
                return;

            this.isUpdatingSelection = true;
            try
            {
                this.siteComboBox.BeginUpdate();
                this.siteComboBox.Items.Clear();
                foreach (RssSiteViewModel rssSiteViewModel in this.viewModel.RssSiteViewModels)
                {
                    this.siteComboBox.Items.Add(rssSiteViewModel);
                }
                this.siteComboBox.EndUpdate();

                RssSiteViewModel? selectedRssSiteViewModel = this.viewModel.SelectedRssSiteViewModel;
                if (selectedRssSiteViewModel != null && this.siteComboBox.Items.Contains(selectedRssSiteViewModel) == true)
                    this.siteComboBox.SelectedItem = selectedRssSiteViewModel;
                else if (this.siteComboBox.Items.Count > 0)
                    this.siteComboBox.SelectedIndex = 0;
                else
                    this.siteComboBox.SelectedIndex = -1;
            }
            finally
            {
                this.isUpdatingSelection = false;
            }

            this.ApplySelectedSite();
        }


        private void RefreshFeedComboBox()
        {
            if (this.IsDisposed == true)
                return;

            this.isUpdatingSelection = true;
            try
            {
                this.feedComboBox.BeginUpdate();
                this.feedComboBox.Items.Clear();

                RssSiteViewModel? rssSiteViewModel = this.viewModel.SelectedRssSiteViewModel;
                if (rssSiteViewModel != null)
                {
                    foreach (RssFeedViewModel rssFeedViewModel in rssSiteViewModel.RssFeedViewModels)
                    {
                        this.feedComboBox.Items.Add(rssFeedViewModel);
                    }
                }

                this.feedComboBox.EndUpdate();

                RssFeedViewModel? selectedRssFeedViewModel = rssSiteViewModel?.SelectedRssFeedViewModel;
                if (selectedRssFeedViewModel != null && this.feedComboBox.Items.Contains(selectedRssFeedViewModel) == true)
                    this.feedComboBox.SelectedItem = selectedRssFeedViewModel;
                else if (this.feedComboBox.Items.Count > 0)
                    this.feedComboBox.SelectedIndex = 0;
                else
                    this.feedComboBox.SelectedIndex = -1;
            }
            finally
            {
                this.isUpdatingSelection = false;
            }

            this.ApplySelectedFeed();
        }


        private void ApplySelectedSite()
        {
            RssSiteViewModel? rssSiteViewModel = this.siteComboBox.SelectedItem as RssSiteViewModel;

            /// مقدار null یعنی «هیچ خبرگزاری انتخاب نشده است» . پروپرتی در ViewModel -
            /// به صورت non-nullable اعلام شده ، ولی در عمل برای حالت بدون انتخاب ، null می‌گیرد .
            if (ReferenceEquals(this.viewModel.SelectedRssSiteViewModel, rssSiteViewModel) == false)
                this.viewModel.SelectedRssSiteViewModel = rssSiteViewModel!;

            /// با تغییر خبرگزاری ، باید به تغییرات کالکشن دسته‌بندی‌های همان خبرگزاری هم گوش بدهیم .
            if (ReferenceEquals(this.observedSite, rssSiteViewModel) == false)
            {
                if (this.observedSite != null)
                    this.observedSite.RssFeedViewModels.CollectionChanged -= this.RssFeedViewModels_CollectionChanged;

                this.observedSite = rssSiteViewModel;

                if (this.observedSite != null)
                    this.observedSite.RssFeedViewModels.CollectionChanged += this.RssFeedViewModels_CollectionChanged;
            }

            this.RefreshFeedComboBox();
        }


        private void ApplySelectedFeed()
        {
            RssSiteViewModel? rssSiteViewModel = this.viewModel.SelectedRssSiteViewModel;
            RssFeedViewModel? rssFeedViewModel = this.feedComboBox.SelectedItem as RssFeedViewModel;

            /// مقدار null یعنی «هیچ دسته‌بندی انتخاب نشده است» . پروپرتی در ViewModel -
            /// به صورت non-nullable اعلام شده ، ولی در عمل برای حالت بدون انتخاب ، null می‌گیرد .
            if (rssSiteViewModel != null && ReferenceEquals(rssSiteViewModel.SelectedRssFeedViewModel, rssFeedViewModel) == false)
                rssSiteViewModel.SelectedRssFeedViewModel = rssFeedViewModel!;

            /// با تغییر دسته‌بندی ، باید به تغییرات کالکشن خبرهای همان دسته‌بندی هم گوش بدهیم .
            if (ReferenceEquals(this.observedFeed, rssFeedViewModel) == false)
            {
                if (this.observedFeed != null)
                    this.observedFeed.RssItemViewModels.CollectionChanged -= this.RssItemViewModels_CollectionChanged;

                this.observedFeed = rssFeedViewModel;

                if (this.observedFeed != null)
                    this.observedFeed.RssItemViewModels.CollectionChanged += this.RssItemViewModels_CollectionChanged;
            }

            this.RebuildItemViews();
            this.SelectedFeedChanged?.Invoke(this, EventArgs.Empty);
        }


        ///////////////////////////////////////////////////////////////////////


        private void SiteComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.isUpdatingSelection == true)
                return;

            this.ApplySelectedSite();
        }


        private void FeedComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.isUpdatingSelection == true)
                return;

            this.ApplySelectedFeed();
        }


        ///////////////////////////////////////////////////////////////////////


        private void RebuildItemViews()
        {
            if (this.IsDisposed == true)
                return;

            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(new Action(this.RebuildItemViews));
                return;
            }

            this.itemsPanel.SuspendLayout();
            try
            {
                foreach (Control control in this.itemsPanel.Controls.Cast<Control>().ToArray())
                {
                    this.itemsPanel.Controls.Remove(control);
                    control.Dispose();
                }

                RssFeedViewModel? rssFeedViewModel = this.viewModel.SelectedRssSiteViewModel?.SelectedRssFeedViewModel;
                if (rssFeedViewModel != null)
                {
                    foreach (RssItemViewModel rssItemViewModel in rssFeedViewModel.RssItemViewModels)
                    {
                        RssItemView rssItemView = new RssItemView(rssItemViewModel)
                        {
                            Width = this.GetItemWidth()
                        };
                        this.itemsPanel.Controls.Add(rssItemView);
                    }
                }
            }
            finally
            {
                this.itemsPanel.ResumeLayout();
            }
        }


        private int GetItemWidth()
        {
            int availableWidth = this.itemsPanel.ClientSize.Width -
                this.itemsPanel.Padding.Horizontal -
                SystemInformation.VerticalScrollBarWidth -
                4;

            return Math.Max(320, availableWidth);
        }


        private void ItemsPanel_SizeChanged(object? sender, EventArgs e)
        {
            int itemWidth = this.GetItemWidth();

            foreach (Control control in this.itemsPanel.Controls)
            {
                if (control.Width != itemWidth)
                    control.Width = itemWidth;
            }
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                this.viewModel.RssSiteViewModels.CollectionChanged -= this.RssSiteViewModels_CollectionChanged;

                if (this.observedSite != null)
                    this.observedSite.RssFeedViewModels.CollectionChanged -= this.RssFeedViewModels_CollectionChanged;

                if (this.observedFeed != null)
                    this.observedFeed.RssItemViewModels.CollectionChanged -= this.RssItemViewModels_CollectionChanged;
            }

            base.Dispose(disposing);
        }

    }
}
