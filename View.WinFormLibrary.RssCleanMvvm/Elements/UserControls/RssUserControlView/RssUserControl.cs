using System.Collections.Specialized;
using View.WinFormLibrary.RssCleanMvvm.AppResource.LanguageResource.Class;
using View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;
using View.WinFormLibrary.RssCleanMvvm.UiLogicModules.BindingModules.Class;
using ViewModel.ViewModelModules.RssFeedViewModelModule.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;
using ViewModel.ViewModelModules.RssSiteViewModelModule.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView
{
    /// <summary>
    /// معادلِ <c>RssUserControl.xaml</c> در نسخه‌ی WPF :
    ///   • انتخاب خبرگزاری (ComboBox اول)
    ///   • انتخاب دسته‌ی خبری (ComboBox دوم)
    ///   • نمایش لیست خبرهای دسته‌ی انتخاب‌شده
    ///
    /// نکته‌ی مهم درباره‌ی بایندینگ :
    /// در WPF مقادیر زیر با Binding انجام می‌شدند :
    ///   ItemsSource = RssSiteViewModels / RssSiteViewModels/RssFeedViewModels
    ///   SelectedItem = SelectedRssSiteViewModel / SelectedRssFeedViewModel
    ///   IsEnabled = SelectedRssSiteViewModel != null
    ///   Command = LoadRssItemsCommand (با Trigger روی SourceUpdated)
    ///
    /// در WinForms، ComboBox برای INotifyCollectionChanged بایندینگ خودکار ندارد؛
    /// بنابراین از <see cref="ViewBinding.BindCollection"/> استفاده می‌کنیم که همان نقش
    /// ItemsSource را دارد (لیست را با تغییر کالکشن هم‌گام می‌کند) . انتخاب‌ها هم از طریق
    /// همان بایندینگ‌های ViewModel هم‌گام می‌شوند . هیچ‌جای این کلاس، منطقِ تجاری وجود ندارد .
    /// </summary>
    public sealed class RssUserControl : UserControl
    {
        private readonly BindingScope bindings;

        private readonly ComboBox siteComboBox;

        private readonly ComboBox feedComboBox;

        private readonly BufferedFlowLayoutPanel itemsPanel;

        private readonly Dictionary<RssItemViewModel, RssItemView> itemViews;

        private RssUserControlViewModel? viewModel;

        private RssSiteViewModel? observedSite;

        private RssFeedViewModel? observedFeed;

        private bool isUpdatingSelection;

        private bool isRssSiteLoadStarted;


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// وقتی دسته‌ی خبریِ انتخاب‌شده عوض شود (یا فیدِ آن عوض شود) اجرا می‌شود .
        /// پنجره‌ی اصلی با این رویداد، Command منوها را دوباره به فیدِ جدید بایند می‌کند .
        /// این یک رویدادِ View است و هیچ وابستگی‌ای به منطق ViewModel ایجاد نمی‌کند .
        /// </summary>
        public event EventHandler? SelectedFeedChanged;


        ///////////////////////////////////////////////////////////////////////


        public RssUserControl()
        {
            this.bindings = new BindingScope();
            this.itemViews = new Dictionary<RssItemViewModel, RssItemView>();

            this.RightToLeft = RightToLeft.Yes;
            this.BackColor = UiTheme.ContentBackground;
            this.Font = UiTheme.BaseFont;
            this.DoubleBuffered = true;

            this.siteComboBox = this.CreateComboBox(nameof(RssSiteViewModel.SiteName));
            this.feedComboBox = this.CreateComboBox(nameof(RssFeedViewModel.Title));

            TableLayoutPanel headerLayout = this.BuildHeaderLayout();

            this.itemsPanel = new BufferedFlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = UiTheme.ContentBackground,
                Padding = new Padding(10, 10, 10, 6),
                RightToLeft = RightToLeft.Yes,
                Margin = new Padding(0)
            };

            /// ترتیب Dock : ابتدا Fill را اضافه می‌کنیم، بعد Top ؛ به این ترتیب هدر بالای
            /// لیست قرار می‌گیرد و لیست، بقیه‌ی فضا را پر می‌کند .
            this.Controls.Add(this.itemsPanel);
            this.Controls.Add(headerLayout);

            this.siteComboBox.SelectedIndexChanged += this.SiteComboBox_SelectedIndexChanged;
            this.feedComboBox.SelectedIndexChanged += this.FeedComboBox_SelectedIndexChanged;
            this.itemsPanel.Scroll += this.ItemsPanel_Scroll;
            this.itemsPanel.Resize += this.ItemsPanel_Resize;
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// اتصالِ View به ViewModel . (معادلِ DataContext در WPF)
        /// این متد را پنجره‌ی اصلی، قبل از نمایش، صدا می‌زند .
        /// </summary>
        public void SetViewModel(RssUserControlViewModel rssUserControlViewModel)
        {
            if (rssUserControlViewModel == null)
                throw new ArgumentNullException(nameof(rssUserControlViewModel));

            if (this.viewModel != null)
                throw new InvalidOperationException("ViewModel قبلاً تنظیم شده است .");

            this.viewModel = rssUserControlViewModel;

            /// انتخابِ خبرگزاری (معادل Binding به SelectedRssSiteViewModel).
            this.bindings.BindOneWay(this, this.viewModel,
                nameof(RssUserControlViewModel.SelectedRssSiteViewModel), this.OnSelectedSiteChanged);

            /// ItemsSource خبرگزاری‌ها (معادل Binding به RssSiteViewModels).
            this.bindings.BindCollection(this, this.viewModel.RssSiteViewModels, this.RefreshSiteComboBox);
        }


        ///////////////////////////////////////////////////////////////////////


        private ComboBox CreateComboBox(string displayMember)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = displayMember,
                Width = 360,
                Font = UiTheme.BaseFont,
                RightToLeft = RightToLeft.Yes,
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.Surface,
                ForeColor = UiTheme.TextPrimary,
                MaxDropDownItems = 12,
                Margin = new Padding(0)
            };
        }


        private TableLayoutPanel BuildHeaderLayout()
        {
            TableLayoutPanel headerLayout = new()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = UiTheme.Surface,
                Padding = new Padding(16, 12, 16, 8),
                Margin = new Padding(0),
                RightToLeft = RightToLeft.Yes
            };

            /// ستون ۰ (لیبل) در حالت RTL سمت راست و ستون ۱ (ComboBox) سمت چپ آن قرار می‌گیرد .
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            this.siteComboBox.Margin = new Padding(0, 6, 0, 6);
            /// در RTL، Anchor چپ/راست هم آینه می‌شود؛ Anchor.Left یعنی «چسبیده به لبه‌ی راستِ سلول»،
            /// یعنی دقیقاً کنار لیبل (که در ستون راست قرار دارد) .
            this.siteComboBox.Anchor = AnchorStyles.Left;

            this.feedComboBox.Margin = new Padding(0, 6, 0, 6);
            this.feedComboBox.Anchor = AnchorStyles.Left;

            headerLayout.Controls.Add(this.CreateHeaderLabel(PersianLanguageResource.NewsSite), 0, 0);
            headerLayout.Controls.Add(this.siteComboBox, 1, 0);
            headerLayout.Controls.Add(this.CreateHeaderLabel(PersianLanguageResource.NewsCategory), 0, 1);
            headerLayout.Controls.Add(this.feedComboBox, 1, 1);

            Panel separator = new()
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = UiTheme.Border,
                Margin = new Padding(0, 8, 0, 0)
            };

            headerLayout.Controls.Add(separator, 0, 2);
            headerLayout.SetColumnSpan(separator, 2);

            return headerLayout;
        }


        private Label CreateHeaderLabel(string labelText)
        {
            return new Label
            {
                Text = labelText,
                AutoSize = true,
                Font = UiTheme.HeaderFont,
                ForeColor = UiTheme.TextSecondary,
                BackColor = Color.Transparent,
                RightToLeft = RightToLeft.Yes,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(0, 0, 14, 0)
            };
        }


        ///////////////////////////////////////////////////////////////////////


        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.viewModel == null || this.isRssSiteLoadStarted == true)
                return;

            this.isRssSiteLoadStarted = true;

            /// معادلِ EventTrigger مربوط به Loaded در نسخه‌ی WPF .
            await this.viewModel.LoadRssSiteCommand.ExecuteAsync(null);
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// ستونِ «خبرگزاری» را با کالکشن هم‌گام می‌کند؛ سپس اگر انتخابی وجود ندارد،
        /// اولین خبرگزاری را انتخاب می‌کند (معادل <c>IsSynchronizedWithCurrentItem=True</c> در WPF).
        /// </summary>
        private void RefreshSiteComboBox()
        {
            if (this.IsDisposed == true || this.viewModel == null)
                return;

            RssSiteViewModel? selectedSite = this.viewModel.SelectedRssSiteViewModel;

            this.isUpdatingSelection = true;
            try
            {
                this.siteComboBox.BeginUpdate();
                this.siteComboBox.Items.Clear();

                foreach (RssSiteViewModel rssSiteViewModel in this.viewModel.RssSiteViewModels)
                    this.siteComboBox.Items.Add(rssSiteViewModel);

                this.siteComboBox.EndUpdate();

                int selectedIndex = selectedSite == null ? -1 : this.siteComboBox.Items.IndexOf(selectedSite);

                if (selectedIndex < 0 && this.siteComboBox.Items.Count > 0)
                    selectedIndex = 0;

                this.siteComboBox.SelectedIndex = selectedIndex;
            }
            finally
            {
                this.isUpdatingSelection = false;
            }

            RssSiteViewModel? siteToApply = this.siteComboBox.SelectedItem as RssSiteViewModel;

            if (siteToApply == null)
            {
                /// کالکشن خالی شده است؛ انتخابِ ViewModel هم باید پاک شود .
                if (this.viewModel.SelectedRssSiteViewModel != null)
                    this.viewModel.SelectedRssSiteViewModel = null!;
                else
                    this.OnSelectedSiteChanged();

                return;
            }

            if (ReferenceEquals(this.viewModel.SelectedRssSiteViewModel, siteToApply) == false)
            {
                /// همین set کردن، باعث اجرای OnSelectedSiteChanged (از طریق بایندینگ) می‌شود .
                this.viewModel.SelectedRssSiteViewModel = siteToApply;
            }
            else
            {
                /// مقدار عوض نشده؛ ولی ممکن است فیدها/آیتم‌ها نیاز به هم‌گام‌سازی داشته باشند .
                this.OnSelectedSiteChanged();
            }
        }


        private void SiteComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.isUpdatingSelection == true || this.viewModel == null)
                return;

            RssSiteViewModel? selectedSite = this.siteComboBox.SelectedItem as RssSiteViewModel;

            if (ReferenceEquals(this.viewModel.SelectedRssSiteViewModel, selectedSite) == false)
                this.viewModel.SelectedRssSiteViewModel = selectedSite!;
        }


        /// <summary>
        /// با تغییر خبرگزاری :
        ///   • به تغییرات SelectedRssFeedViewModel و کالکشن فیدهای همان خبرگزاری گوش می‌دهیم .
        ///   • ComboBox دسته‌ها را دوباره پر می‌کنیم .
        ///   • لیست آیتم‌ها را با فیدِ جدید هم‌گام می‌کنیم .
        /// </summary>
        private void OnSelectedSiteChanged()
        {
            if (this.viewModel == null || this.IsDisposed == true)
                return;

            RssSiteViewModel? selectedSite = this.viewModel.SelectedRssSiteViewModel;

            if (ReferenceEquals(this.observedSite, selectedSite) == false)
            {
                if (this.observedSite != null)
                {
                    this.observedSite.PropertyChanged -= this.ObservedSite_PropertyChanged;
                    this.observedSite.RssFeedViewModels.CollectionChanged -= this.ObservedSite_RssFeedViewModels_CollectionChanged;
                }

                this.observedSite = selectedSite;

                if (this.observedSite != null)
                {
                    this.observedSite.PropertyChanged += this.ObservedSite_PropertyChanged;
                    this.observedSite.RssFeedViewModels.CollectionChanged += this.ObservedSite_RssFeedViewModels_CollectionChanged;
                }
            }

            /// ComboBox را با انتخابِ ViewModel هم‌گام می‌کنیم (بدون ایجاد حلقه).
            this.isUpdatingSelection = true;
            try
            {
                int selectedIndex = selectedSite == null ? -1 : this.siteComboBox.Items.IndexOf(selectedSite);
                if (this.siteComboBox.SelectedIndex != selectedIndex)
                    this.siteComboBox.SelectedIndex = selectedIndex;
            }
            finally
            {
                this.isUpdatingSelection = false;
            }

            this.feedComboBox.Enabled = selectedSite != null;

            this.RefreshFeedComboBox();
        }


        private void ObservedSite_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(RssSiteViewModel.SelectedRssFeedViewModel))
                return;

            this.SynchronizeFeedSelection();
            this.OnSelectedFeedChanged(scrollToTop: true);
        }


        private void ObservedSite_RssFeedViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this.RefreshFeedComboBox();
        }


        private void RefreshFeedComboBox()
        {
            if (this.IsDisposed == true || this.viewModel == null)
                return;

            RssSiteViewModel? selectedSite = this.viewModel.SelectedRssSiteViewModel;

            this.isUpdatingSelection = true;
            try
            {
                this.feedComboBox.BeginUpdate();
                this.feedComboBox.Items.Clear();

                if (selectedSite != null)
                {
                    foreach (RssFeedViewModel rssFeedViewModel in selectedSite.RssFeedViewModels)
                        this.feedComboBox.Items.Add(rssFeedViewModel);
                }

                this.feedComboBox.EndUpdate();

                /// در WPF هم به‌صورت خودکار اولین دسته انتخاب نمی‌شد؛
                /// انتخاب فقط وقتی است که ViewModel از قبل فیدی انتخاب کرده باشد .
                int selectedIndex = -1;
                if (selectedSite?.SelectedRssFeedViewModel != null)
                    selectedIndex = this.feedComboBox.Items.IndexOf(selectedSite.SelectedRssFeedViewModel);

                this.feedComboBox.SelectedIndex = selectedIndex;
                this.feedComboBox.Enabled = selectedSite != null;
            }
            finally
            {
                this.isUpdatingSelection = false;
            }

            this.OnSelectedFeedChanged(scrollToTop: true);
        }


        private void SynchronizeFeedSelection()
        {
            RssSiteViewModel? selectedSite = this.viewModel?.SelectedRssSiteViewModel;

            int selectedIndex = -1;
            if (selectedSite?.SelectedRssFeedViewModel != null)
                selectedIndex = this.feedComboBox.Items.IndexOf(selectedSite.SelectedRssFeedViewModel);

            if (this.feedComboBox.SelectedIndex == selectedIndex)
                return;

            this.isUpdatingSelection = true;
            try
            {
                this.feedComboBox.SelectedIndex = selectedIndex;
            }
            finally
            {
                this.isUpdatingSelection = false;
            }
        }


        private void FeedComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.isUpdatingSelection == true || this.viewModel == null)
                return;

            RssSiteViewModel? selectedSite = this.viewModel.SelectedRssSiteViewModel;
            if (selectedSite == null)
                return;

            RssFeedViewModel? selectedFeed = this.feedComboBox.SelectedItem as RssFeedViewModel;

            if (ReferenceEquals(selectedSite.SelectedRssFeedViewModel, selectedFeed) == false)
                selectedSite.SelectedRssFeedViewModel = selectedFeed!;

            this.OnSelectedFeedChanged(scrollToTop: true);

            /// در WPF این کار با RootedEventTrigger روی Binding.SourceUpdated انجام می‌شد :
            /// با انتخاب دسته، آیتم‌های آن Load می‌شوند .
            if (selectedFeed != null && selectedFeed.LoadRssItemsCommand.CanExecute(null) == true)
                _ = selectedFeed.LoadRssItemsCommand.ExecuteAsync(null);
        }


        private void OnSelectedFeedChanged(bool scrollToTop)
        {
            if (this.viewModel == null || this.IsDisposed == true)
                return;

            RssFeedViewModel? selectedFeed = this.viewModel.SelectedRssSiteViewModel?.SelectedRssFeedViewModel;

            if (ReferenceEquals(this.observedFeed, selectedFeed) == false)
            {
                if (this.observedFeed != null)
                    this.observedFeed.RssItemViewModels.CollectionChanged -= this.ObservedFeed_RssItemViewModels_CollectionChanged;

                this.observedFeed = selectedFeed;

                if (this.observedFeed != null)
                    this.observedFeed.RssItemViewModels.CollectionChanged += this.ObservedFeed_RssItemViewModels_CollectionChanged;
            }

            this.SynchronizeItemViews(scrollToTop);
            this.SelectedFeedChanged?.Invoke(this, EventArgs.Empty);
        }


        private void ObservedFeed_RssItemViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            /// کالکشن ممکن است پس از await (و بنابراین روی نخ UI) تغییر کند، ولی برای اطمینان
            /// به نخ UI منتقل می‌شود (همان کاری که WPF با Dispatcher انجام می‌دهد) .
            UiThread.Post(this, () => this.SynchronizeItemViews(scrollToTop: false));
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// لیست آیتم‌ها را با کالکشن فیدِ انتخاب‌شده هم‌گام می‌کند .
        ///
        /// چرا به‌جای ساخت دوباره‌ی همه‌ی کنترل‌ها، «هم‌گام‌سازی» می‌کنیم؟
        /// هر RssItemView چندین کنترل و تصویر دارد؛ ساختن دوباره‌ی آن‌ها در هر تغییرِ کالکشن
        /// هم کند است و هم باعث پرش/چشمک می‌شود . فقط آیتم‌های جدید ساخته و آیتم‌های حذف‌شده
        /// Dispose می‌شوند؛ بقیه، همان کنترل قبلی را نگه می‌دارند .
        /// </summary>
        private void SynchronizeItemViews(bool scrollToTop)
        {
            if (this.IsDisposed == true || this.viewModel == null)
                return;

            IEnumerable<RssItemViewModel> currentItems = this.viewModel.SelectedRssSiteViewModel?.SelectedRssFeedViewModel?.RssItemViewModels
                ?? Enumerable.Empty<RssItemViewModel>();

            HashSet<RssItemViewModel> currentItemSet = new(currentItems);

            /// حذفِ کنترل‌های آیتم‌هایی که دیگر در کالکشن نیستند .
            foreach (KeyValuePair<RssItemViewModel, RssItemView> pair in this.itemViews.ToArray())
            {
                if (currentItemSet.Contains(pair.Key) == true)
                    continue;

                this.itemsPanel.Controls.Remove(pair.Value);
                pair.Value.Dispose();
                this.itemViews.Remove(pair.Key);
            }

            int itemWidth = this.GetItemWidth();

            this.itemsPanel.SuspendLayout();
            try
            {
                int index = 0;

                foreach (RssItemViewModel rssItemViewModel in currentItems)
                {
                    if (this.itemViews.TryGetValue(rssItemViewModel, out RssItemView? itemView) == false)
                    {
                        itemView = new RssItemView(rssItemViewModel);
                        this.itemViews.Add(rssItemViewModel, itemView);
                        this.itemsPanel.Controls.Add(itemView);
                    }

                    if (itemView.Width != itemWidth)
                        itemView.Width = itemWidth;

                    this.itemsPanel.Controls.SetChildIndex(itemView, index);
                    index++;
                }
            }
            finally
            {
                this.itemsPanel.ResumeLayout(true);
            }

            if (scrollToTop == true)
                this.itemsPanel.AutoScrollPosition = new Point(0, 0);

            this.UpdateVisibleItemImages();
        }


        private int GetItemWidth()
        {
            int availableWidth = this.itemsPanel.ClientSize.Width -
                                 this.itemsPanel.Padding.Horizontal -
                                 SystemInformation.VerticalScrollBarWidth -
                                 2;

            return Math.Max(420, availableWidth);
        }


        private void ItemsPanel_Resize(object? sender, EventArgs e)
        {
            int itemWidth = this.GetItemWidth();

            foreach (RssItemView itemView in this.itemViews.Values)
            {
                if (itemView.Width != itemWidth)
                    itemView.Width = itemWidth;
            }

            this.UpdateVisibleItemImages();
        }


        private void ItemsPanel_Scroll(object? sender, ScrollEventArgs e)
        {
            this.UpdateVisibleItemImages();
        }


        /// <summary>
        /// تصویرِ آیتم‌ها فقط وقتی بارگذاری می‌شود که در ناحیه‌ی دید باشند .
        /// این کار، مصرف حافظه و زمانِ اولیه را برای لیست‌های طولانی کم می‌کند .
        /// </summary>
        private void UpdateVisibleItemImages()
        {
            if (this.IsDisposed == true)
                return;

            Rectangle viewport = this.itemsPanel.ClientRectangle;

            foreach (Control control in this.itemsPanel.Controls)
            {
                if (control is RssItemView itemView)
                    itemView.SetViewportVisible(viewport.IntersectsWith(itemView.Bounds) == true);
            }
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                this.bindings.Dispose();

                if (this.observedSite != null)
                {
                    this.observedSite.PropertyChanged -= this.ObservedSite_PropertyChanged;
                    this.observedSite.RssFeedViewModels.CollectionChanged -= this.ObservedSite_RssFeedViewModels_CollectionChanged;
                }

                if (this.observedFeed != null)
                    this.observedFeed.RssItemViewModels.CollectionChanged -= this.ObservedFeed_RssItemViewModels_CollectionChanged;

                foreach (RssItemView itemView in this.itemViews.Values)
                    itemView.Dispose();

                this.itemViews.Clear();
            }

            base.Dispose(disposing);
        }

    }
}
