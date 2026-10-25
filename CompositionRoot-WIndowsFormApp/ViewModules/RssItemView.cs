using System.ComponentModel;
using ViewModel.ViewModelModules.RssItemViewModelModule.Class;

namespace CompositionRoot.WIndowsFormApp.ViewModules
{
    /// <summary>
    /// نمایش و ویرایش یک خبر ، معادل DataTemplate مربوط به RssItemViewModel در نسخه‌ی WPF .
    /// </summary>
    internal sealed class RssItemView : UserControl
    {

        private readonly RssItemViewModel viewModel;

        private readonly Label titleLabel;

        private readonly Label categoryLabel;

        private readonly Label descriptionLabel;

        private readonly TextBox descriptionTextBox;

        private readonly Label publishDateLabel;

        private readonly PictureBox imageBox;

        private readonly CheckBox editModeCheckBox;

        private readonly Button convertToGrayscaleButton;

        private readonly Button saveButton;

        private bool isSynchronizingEditState;


        ///////////////////////////////////////////////////////////////////////


        public RssItemView(RssItemViewModel viewModel)
        {
            this.viewModel = viewModel;

            this.RightToLeft = RightToLeft.Yes;
            this.BackColor = Color.White;
            this.Margin = new Padding(0, 0, 0, 6);
            this.Padding = new Padding(4);
            this.Height = 200;
            this.DoubleBuffered = true;

            this.titleLabel = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 25),
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            this.categoryLabel = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(90, 90, 90),
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            this.descriptionLabel = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.TopRight,
                Padding = new Padding(0, 4, 0, 0),
                ForeColor = Color.FromArgb(45, 45, 45),
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            this.descriptionTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                AcceptsReturn = true,
                ScrollBars = ScrollBars.Vertical,
                RightToLeft = RightToLeft.Yes,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Margin = new Padding(0)
            };

            this.publishDateLabel = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(110, 110, 110),
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            this.imageBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(10, 0, 10, 0)
            };

            this.editModeCheckBox = new CheckBox
            {
                Appearance = Appearance.Button,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "ویرایش",
                FlatStyle = FlatStyle.Standard,
                Margin = new Padding(0)
            };

            this.convertToGrayscaleButton = new Button
            {
                Dock = DockStyle.Fill,
                Text = "تبدیل به سیاه و سفید",
                Margin = new Padding(0, 6, 0, 0)
            };

            this.saveButton = new Button
            {
                Dock = DockStyle.Fill,
                Text = "ذخیره",
                Enabled = false,
                Margin = new Padding(0, 6, 0, 0)
            };

            this.Controls.Add(this.BuildLayout());

            this.BindViewModel();

            this.editModeCheckBox.CheckedChanged += this.EditModeCheckBox_CheckedChanged;
            this.descriptionTextBox.Leave += this.DescriptionTextBox_Leave;
            this.convertToGrayscaleButton.Click += this.ConvertToGrayscaleButton_Click;

            this.SynchronizeEditState();
            this.SynchronizeUnsavedState();
            this.SynchronizeImage();
        }


        ///////////////////////////////////////////////////////////////////////


        private Control BuildLayout()
        {
            Panel descriptionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };
            /// ابتدا Label و بعد TextBox اضافه می‌شود . در هر لحظه فقط یکی از آن‌ها Visible است .
            descriptionPanel.Controls.Add(this.descriptionLabel);
            descriptionPanel.Controls.Add(this.descriptionTextBox);

            TableLayoutPanel infoLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            infoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            infoLayout.Controls.Add(this.titleLabel, 0, 0);
            infoLayout.Controls.Add(this.categoryLabel, 0, 1);
            infoLayout.Controls.Add(descriptionPanel, 0, 2);
            infoLayout.Controls.Add(this.publishDateLabel, 0, 3);

            TableLayoutPanel actionsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            actionsLayout.Controls.Add(this.editModeCheckBox, 0, 0);
            actionsLayout.Controls.Add(this.convertToGrayscaleButton, 0, 1);
            actionsLayout.Controls.Add(this.saveButton, 0, 2);

            TableLayoutPanel rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(6),
                BackColor = Color.Transparent
            };
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 124F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(infoLayout, 0, 0);
            rootLayout.Controls.Add(this.imageBox, 1, 0);
            rootLayout.Controls.Add(actionsLayout, 2, 0);

            return rootLayout;
        }


        private void BindViewModel()
        {
            this.titleLabel.DataBindings.Add(nameof(Label.Text), this.viewModel,
                nameof(RssItemViewModel.Title), true, DataSourceUpdateMode.Never);

            this.categoryLabel.DataBindings.Add(nameof(Label.Text), this.viewModel,
                nameof(RssItemViewModel.Category), true, DataSourceUpdateMode.Never, string.Empty,
                "در دسته بندی : {0}");

            this.descriptionLabel.DataBindings.Add(nameof(Label.Text), this.viewModel,
                nameof(RssItemViewModel.Description), true, DataSourceUpdateMode.Never);

            /// معادل TextBox موجود در WPF که با Mode=TwoWay به Description بایند شده است .
            this.descriptionTextBox.DataBindings.Add(nameof(TextBox.Text), this.viewModel,
                nameof(RssItemViewModel.Description), true, DataSourceUpdateMode.OnPropertyChanged);

            this.publishDateLabel.DataBindings.Add(nameof(Label.Text), this.viewModel,
                nameof(RssItemViewModel.PersianPublishDate), true, DataSourceUpdateMode.Never);

            this.viewModel.PropertyChanged += this.RssItemViewModel_PropertyChanged;
        }


        ///////////////////////////////////////////////////////////////////////


        private void RssItemViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(RssItemViewModel.IsInEditMode):
                    UiThread.Post(this, this.SynchronizeEditState);
                    break;

                case nameof(RssItemViewModel.ImageFullFilePath):
                    UiThread.Post(this, this.SynchronizeImage);
                    break;

                case nameof(RssItemViewModel.IsUnsaved):
                    UiThread.Post(this, this.SynchronizeUnsavedState);
                    break;
            }
        }


        private void SynchronizeEditState()
        {
            this.isSynchronizingEditState = true;
            try
            {
                this.editModeCheckBox.Checked = this.viewModel.IsInEditMode;
                this.editModeCheckBox.Text = this.viewModel.IsInEditMode ? "پایان ویرایش" : "ویرایش";

                this.descriptionLabel.Visible = this.viewModel.IsInEditMode == false;
                this.descriptionTextBox.Visible = this.viewModel.IsInEditMode;
            }
            finally
            {
                this.isSynchronizingEditState = false;
            }
        }


        private void SynchronizeUnsavedState()
        {
            /// خبرهایی که تغییر ذخیره‌نشده دارند ، با پس‌زمینه‌ی ملایم متمایز می‌شوند .
            this.BackColor = this.viewModel.IsUnsaved == true
                ? Color.FromArgb(255, 248, 220)
                : Color.White;

            this.titleLabel.ForeColor = this.viewModel.IsUnsaved == true
                ? Color.FromArgb(140, 70, 0)
                : Color.FromArgb(25, 25, 25);
        }


        private void SynchronizeImage()
        {
            Image? previousImage = this.imageBox.Image;
            Image? newImage = LoadImage(this.viewModel.ImageFullFilePath);

            this.imageBox.Image = newImage;
            this.imageBox.Visible = newImage != null;

            previousImage?.Dispose();
        }


        private static Image? LoadImage(string? fullFilePath)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath) == true || File.Exists(fullFilePath) == false)
                return null;

            try
            {
                /// فایل را با FileShare.ReadWrite باز می‌کنیم تا تبدیل تصویر به سیاه و سفید -
                /// که همان فایل را بازنویسی می‌کند ، با خطای قفل فایل مواجه نشود .
                using FileStream stream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using Image sourceImage = Image.FromStream(stream);
                return new Bitmap(sourceImage);
            }
            catch (Exception)
            {
                return null;
            }
        }


        ///////////////////////////////////////////////////////////////////////


        private void EditModeCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (this.isSynchronizingEditState == true)
                return;

            this.viewModel.IsInEditMode = this.editModeCheckBox.Checked;
        }


        private async void DescriptionTextBox_Leave(object? sender, EventArgs e)
        {
            if (this.viewModel.IsInEditMode == false)
                return;

            try
            {
                /// معادل Trigger رویداد Binding.SourceUpdated در WPF :
                /// بعد از خروج کاربر از TextBox ، متن ویرایش‌شده در کش موقت ذخیره می‌شود .
                if (this.viewModel.SaveRssItemDataToDraftCacheCommand.CanExecute(null) == true)
                    await this.viewModel.SaveRssItemDataToDraftCacheCommand.ExecuteAsync(null);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.ToString(), "خطا در ذخیره‌ی متن خبر",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            }
        }


        private async void ConvertToGrayscaleButton_Click(object? sender, EventArgs e)
        {
            this.convertToGrayscaleButton.Enabled = false;
            try
            {
                await this.viewModel.ConvertRssItemImageToGrayscaleCommand.ExecuteAsync(null);
            }
            catch (Exception exception)
            {
                if (this.IsDisposed == false)
                {
                    MessageBox.Show(this, exception.ToString(), "خطا در تبدیل تصویر به سیاه و سفید",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            }
            finally
            {
                if (this.IsDisposed == false)
                    this.convertToGrayscaleButton.Enabled = true;
            }
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                this.viewModel.PropertyChanged -= this.RssItemViewModel_PropertyChanged;

                Image? image = this.imageBox.Image;
                this.imageBox.Image = null;
                image?.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
