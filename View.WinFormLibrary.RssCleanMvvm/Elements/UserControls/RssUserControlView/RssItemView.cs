using View.WinFormLibrary.RssCleanMvvm.AppResource.LanguageResource.Class;
using View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class;
using View.WinFormLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView.BindingConverterModules.FilePathToImageConverterModule;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;
using View.WinFormLibrary.RssCleanMvvm.UiLogicModules.BindingModules.Class;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView
{
    /// <summary>
    /// نمایش و ویرایش یک خبر ، معادلِ <c>DataTemplate</c> مربوط به <see cref="RssItemViewModel"/>
    /// در نسخه‌ی WPF .
    ///
    /// ساختار بصری (هم‌ترتیب با WPF) :
    ///   ستون ۱ : عنوان / دسته‌بندی / متن خبر (Label یا TextBox بسته به IsInEditMode) / تاریخ
    ///   ستون ۲ : تصویر خبر (۱۹۲×۱۰۸)
    ///   ستون ۳ : دکمه‌ی حالت ویرایش / تبدیل به سیاه‌وسفید / ذخیره
    ///
    /// همه‌ی ارتباط‌ها با ViewModel از طریق بایندینگ است :
    ///   • متن‌ها با <see cref="ViewBinding.BindText"/> (DataBindings خود WinForms)
    ///   • وضعیت ویرایش / تصویر / رنگِ ذخیره‌نشده با <see cref="ViewBinding.BindOneWay"/>
    ///   • دکمه‌ها با <see cref="ViewBinding.BindCommand"/> (Enabled = CanExecute)
    ///
    /// در این فایل هیچ متدی از ViewModel وجود ندارد که «منطق تجاری» را انجام دهد؛
    /// فقط Command ها اجرا می‌شوند و Propertyها خوانده/نوشته می‌شوند .
    /// </summary>
    public sealed class RssItemView : UserControl
    {
        private readonly RssItemViewModel viewModel;

        private readonly BindingScope bindings;

        private readonly TableLayoutPanel rootLayout;

        private readonly WrappedLabel titleLabel;

        private readonly WrappedLabel categoryLabel;

        private readonly WrappedLabel descriptionLabel;

        private readonly TextBox descriptionTextBox;

        private readonly WrappedLabel publishDateLabel;

        private readonly PictureBox imageBox;

        private readonly RoundedToggleButton editModeToggle;

        private readonly RoundedButton convertToGrayScaleButton;

        private readonly RoundedButton saveButton;

        private bool isSynchronizingEditMode;

        private bool isDescriptionDirty;

        private bool isViewportVisible;

        private bool isImageDirty;

        private Image? displayedImage;


        ///////////////////////////////////////////////////////////////////////


        public RssItemView(RssItemViewModel viewModel)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw, true);

            this.bindings = new BindingScope();

            this.RightToLeft = RightToLeft.Yes;
            this.BackColor = UiTheme.Surface;
            this.Padding = new Padding(14, 10, 14, 12);
            this.Margin = new Padding(0, 0, 0, 8);
            this.Width = 900;
            this.Height = 220;
            this.Font = UiTheme.BaseFont;

            this.titleLabel = new WrappedLabel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Font = UiTheme.TitleFont,
                ForeColor = UiTheme.TextPrimary,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 2)
            };

            this.categoryLabel = new WrappedLabel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Wrap = false,
                Font = UiTheme.CategoryFont,
                ForeColor = UiTheme.TextSecondary,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 2, 0, 2)
            };

            this.descriptionLabel = new WrappedLabel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Font = UiTheme.BaseFont,
                ForeColor = UiTheme.TextPrimary,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 8, 0, 4)
            };

            this.descriptionTextBox = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Multiline = true,
                AcceptsReturn = true,
                WordWrap = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                RightToLeft = RightToLeft.Yes,
                Font = UiTheme.BaseFont,
                BackColor = UiTheme.Surface,
                ForeColor = UiTheme.TextPrimary,
                Height = 110,
                Visible = false,
                Margin = new Padding(0, 8, 0, 4)
            };

            this.publishDateLabel = new WrappedLabel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Wrap = false,
                Font = UiTheme.DateFont,
                ForeColor = UiTheme.TextSecondary,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 2, 0, 0)
            };

            this.imageBox = new PictureBox
            {
                Size = new Size(UiTheme.ItemImageWidth, UiTheme.ItemImageHeight),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = UiTheme.ImagePlaceholder,
                Anchor = AnchorStyles.None,
                Margin = new Padding(22, 0, 22, 0)
            };

            this.editModeToggle = new RoundedToggleButton
            {
                Text = PersianLanguageResource.EditMode,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 0)
            };

            this.convertToGrayScaleButton = new RoundedButton
            {
                Text = PersianLanguageResource.ConvertToGrayScale,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 0)
            };

            this.saveButton = new RoundedButton
            {
                Text = PersianLanguageResource.Save,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 0, 0)
            };

            this.rootLayout = this.BuildRootLayout();

            this.Controls.Add(this.rootLayout);

            this.rootLayout.SizeChanged += this.RootLayout_SizeChanged;

            this.BindViewModel();

            this.SynchronizeEditMode();
            this.AdjustHeight();
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// به پنل لیست می‌گوید این آیتم الان در ناحیه‌ی دید است یا نه .
        /// تصویر فقط وقتی بارگذاری می‌شود که آیتم دیده شود (مثل Virtualization در WPF،
        /// ولی به‌صورت بسیار ساده‌تر) .
        /// </summary>
        public void SetViewportVisible(bool isViewportVisible)
        {
            this.isViewportVisible = isViewportVisible;

            if (isViewportVisible == true)
                this.EnsureImageLoaded();
        }


        ///////////////////////////////////////////////////////////////////////


        private TableLayoutPanel BuildRootLayout()
        {
            TableLayoutPanel actionsLayout = new()
            {
                AutoSize = false,
                ColumnCount = 1,
                RowCount = 3,
                Width = UiTheme.ItemActionColumnWidth,
                Height = 110,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Anchor = AnchorStyles.None,
                BackColor = Color.Transparent
            };
            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            actionsLayout.Controls.Add(this.editModeToggle, 0, 0);
            actionsLayout.Controls.Add(this.convertToGrayScaleButton, 0, 1);
            actionsLayout.Controls.Add(this.saveButton, 0, 2);

            TableLayoutPanel root = new()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                RowCount = 5,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.Transparent,
                RightToLeft = RightToLeft.Yes
            };

            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            for (int rowIndex = 0; rowIndex < 5; rowIndex++)
                root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            /// ترتیب ستون‌ها در حالت RTL توسط TableLayoutPanel آینه می‌شود :
            /// ستون ۰ (متن) سمت راست، ستون ۱ (تصویر) وسط، ستون ۲ (دکمه‌ها) سمت چپ .
            root.Controls.Add(this.titleLabel, 0, 0);
            root.Controls.Add(this.categoryLabel, 0, 1);
            root.Controls.Add(this.descriptionLabel, 0, 2);
            root.Controls.Add(this.descriptionTextBox, 0, 3);
            root.Controls.Add(this.publishDateLabel, 0, 4);
            root.Controls.Add(this.imageBox, 1, 0);
            root.Controls.Add(actionsLayout, 2, 0);

            root.SetRowSpan(this.imageBox, 5);
            root.SetRowSpan(actionsLayout, 5);

            return root;
        }


        private void BindViewModel()
        {
            /// ===== متن‌ها =====
            this.bindings.BindText(this.titleLabel, this.viewModel, nameof(RssItemViewModel.Title));
            this.bindings.BindText(this.categoryLabel, this.viewModel, nameof(RssItemViewModel.Category),
                PersianLanguageResource.CategoryFormat);
            this.bindings.BindText(this.descriptionLabel, this.viewModel, nameof(RssItemViewModel.Description));
            this.bindings.BindText(this.descriptionTextBox, this.viewModel, nameof(RssItemViewModel.Description),
                twoWay: true);
            this.bindings.BindText(this.publishDateLabel, this.viewModel, nameof(RssItemViewModel.PersianPublishDate));

            /// مقدار اولیه‌ی TextBox که هنگام افزودن Binding ست می‌شود، نباید «تغییر کاربر» حساب شود .
            this.isDescriptionDirty = false;

            /// ===== وضعیت حالت ویرایش =====
            this.bindings.BindOneWay(this, this.viewModel, nameof(RssItemViewModel.IsInEditMode),
                this.SynchronizeEditMode);

            /// ===== تصویر =====
            this.bindings.BindOneWay(this, this.viewModel, nameof(RssItemViewModel.ImageFullFilePath),
                this.OnImagePathChanged);

            /// ===== رنگِ حاشیه‌ی پایین (ذخیره‌نشده = قرمز) =====
            this.bindings.BindOneWay(this, this.viewModel, nameof(RssItemViewModel.IsUnsaved),
                this.Invalidate);

            /// ===== Command ها =====
            this.bindings.BindCommand(this.convertToGrayScaleButton,
                this.viewModel.ConvertRssItemImageToGrayscaleCommand);

            this.bindings.BindCommand(this.saveButton, this.viewModel.CommitRssItemCommand);

            /// ===== رویدادهای View =====
            this.editModeToggle.CheckedChanged += this.EditModeToggle_CheckedChanged;
            this.bindings.Add(() => this.editModeToggle.CheckedChanged -= this.EditModeToggle_CheckedChanged);

            this.descriptionTextBox.TextChanged += this.DescriptionTextBox_TextChanged;
            this.bindings.Add(() => this.descriptionTextBox.TextChanged -= this.DescriptionTextBox_TextChanged);

            this.descriptionTextBox.Leave += this.DescriptionTextBox_Leave;
            this.bindings.Add(() => this.descriptionTextBox.Leave -= this.DescriptionTextBox_Leave);
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// همان کاری که DataTrigger (روی IsInEditMode) در WPF انجام می‌داد :
        ///   • متنِ ToggleButton : «حالت ویرایش» ↔ «حالت خواندن»
        ///   • نمایشِ TextBlock (خواندن) ↔ TextBox (ویرایش)
        /// </summary>
        private void SynchronizeEditMode()
        {
            bool isInEditMode = this.viewModel.IsInEditMode;

            this.isSynchronizingEditMode = true;
            try
            {
                this.editModeToggle.Checked = isInEditMode;
                this.editModeToggle.Text = isInEditMode == true
                    ? PersianLanguageResource.ReadMode
                    : PersianLanguageResource.EditMode;

                /// اگر کاربر در حال ویرایش، حالت را به «خواندن» تغییر دهد و به هر دلیلی
                /// Leave اجرا نشده باشد، اینجا تغییرِ ذخیره‌نشده را در کش موقت ذخیره می‌کنیم .
                if (isInEditMode == false && this.isDescriptionDirty == true)
                    _ = this.SaveDescriptionToDraftCacheAsync();

                this.descriptionLabel.Visible = isInEditMode == false;
                this.descriptionTextBox.Visible = isInEditMode;
            }
            finally
            {
                this.isSynchronizingEditMode = false;
            }

            /// با عوض شدن Visible، ارتفاعِ ردیفِ متن هم عوض می‌شود .
            this.rootLayout.PerformLayout();
        }


        private void EditModeToggle_CheckedChanged(object? sender, EventArgs e)
        {
            if (this.isSynchronizingEditMode == true)
                return;

            this.viewModel.IsInEditMode = this.editModeToggle.Checked;
        }


        private void DescriptionTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (this.isSynchronizingEditMode == true)
                return;

            this.isDescriptionDirty = true;
        }


        /// <summary>
        /// معادلِ Trigger مربوط به <c>Binding.SourceUpdated</c> در نسخه‌ی WPF :
        /// وقتی کاربر از TextBox خارج می‌شود، متنِ ویرایش‌شده در کش موقت ذخیره می‌شود .
        ///
        /// در WinForms، DataBindings با <c>OnPropertyChanged</c> متن را در همان لحظه‌ی
        /// تایپ به ViewModel می‌فرستد؛ پس در اینجا فقط Command ذخیره را اجرا می‌کنیم .
        /// </summary>
        private async void DescriptionTextBox_Leave(object? sender, EventArgs e)
        {
            /// اگر TextBox به‌صورت برنامه‌ای پنهان شده باشد (تغییر حالت ویرایش)،
            /// SynchronizeEditMode خودش ذخیره را انجام می‌دهد .
            if (this.descriptionTextBox.Visible == false)
                return;

            await this.SaveDescriptionToDraftCacheAsync();
        }


        /// <summary>
        /// متنِ ویرایش‌شده را در کش موقت ذخیره می‌کند (اجرای Command ذخیره).
        /// در WPF این کار با Trigger روی SourceUpdated انجام می‌شد؛ در WinForms
        /// Valuebinding خودکار همان لحظه انجام می‌شود، پس فقط Command را صدا می‌زنیم .
        /// </summary>
        private async Task SaveDescriptionToDraftCacheAsync()
        {
            if (this.isDescriptionDirty == false)
                return;

            this.isDescriptionDirty = false;

            try
            {
                if (this.viewModel.SaveRssItemDataToDraftCacheCommand.CanExecute(null) == true)
                    await this.viewModel.SaveRssItemDataToDraftCacheCommand.ExecuteAsync(null);
            }
            catch (Exception exception)
            {
                if (this.IsDisposed == false)
                {
                    MessageBox.Show(this, exception.ToString(), PersianLanguageResource.ErrorCaption,
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////


        private void RootLayout_SizeChanged(object? sender, EventArgs e)
        {
            this.AdjustHeight();
        }


        private void AdjustHeight()
        {
            /// ارتفاعِ آیتم = ارتفاعِ محتوای جدول + Padding بالا/پایین + یک پیکسل برای خطِ پایین .
            int newHeight = this.rootLayout.Height + this.Padding.Vertical + 1;

            if (newHeight < 1 || this.Height == newHeight)
                return;

            this.Height = newHeight;
        }


        ///////////////////////////////////////////////////////////////////////


        private void OnImagePathChanged()
        {
            this.isImageDirty = true;

            /// اگر آیتم الان در ناحیه‌ی دید است، همان لحظه تصویر جدید را بارگذاری کن .
            /// (مثلاً بعد از تبدیل به سیاه و سفید، مسیر همان است ولی محتوای فایل عوض شده)
            if (this.isViewportVisible == true)
                this.EnsureImageLoaded();

            this.Invalidate();
        }


        private void EnsureImageLoaded()
        {
            if (this.isImageDirty == false)
                return;

            this.isImageDirty = false;

            Image? newImage = FilePathToImageConverter.Convert(this.viewModel.ImageFullFilePath,
                UiTheme.ItemImageWidth, UiTheme.ItemImageHeight);

            Image? oldImage = this.displayedImage;
            this.displayedImage = newImage;
            this.imageBox.Image = newImage;
            oldImage?.Dispose();
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            /// خطِ پایینِ آیتم : قرمز برای ذخیره‌نشده، خاکستریِ روشن برای ذخیره‌شده .
            /// (معادل BorderBrush در DataTemplate نسخه‌ی WPF)
            using Pen borderPen = new(this.viewModel.IsUnsaved == true ? UiTheme.UnsavedBorder : UiTheme.SavedBorder, 1F);
            e.Graphics.DrawLine(borderPen, 0, this.ClientSize.Height - 1,
                this.ClientSize.Width, this.ClientSize.Height - 1);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                this.bindings.Dispose();

                this.imageBox.Image = null;
                this.displayedImage?.Dispose();
                this.displayedImage = null;
            }

            base.Dispose(disposing);
        }

    }
}
