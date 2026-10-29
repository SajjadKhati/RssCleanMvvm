using View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.Windows.WaitingWindowView
{
    /// <summary>
    /// پنجره‌ی انتظار، معادلِ <c>WaitingWindow</c> در نسخه‌ی WPF .
    ///
    /// مانند WPF :
    ///   • بدون حاشیه و بدون حضور در Taskbar است .
    ///   • نسبت به پنجره‌ی Owner وسط‌چین می‌شود .
    ///   • اسپینرِ چرخان + متنِ انتظار دارد .
    ///
    /// تفاوتِ عمدی: انیمیشن با یک Timer و رسمِ کمان انجام می‌شود (چون Storyboard وجود ندارد) .
    /// </summary>
    public sealed class WaitingWindow : Form
    {
        private readonly LoadingSpinner spinner;

        private readonly Label waitingTextLabel;

        /// <summary>
        /// فقط WaitingDialog اجازه دارد این پنجره را ببندد؛ کاربر (Alt+F4) نباید بتواند .
        /// </summary>
        private bool isCloseRequestedByOwner;


        ///////////////////////////////////////////////////////////////////////


        public WaitingWindow() : this("در حال بارگذاری ...")
        {
        }


        public WaitingWindow(string waitingText)
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            this.Text = waitingText;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.BackColor = UiTheme.Surface;
            this.RightToLeft = RightToLeft.Yes;
            this.ClientSize = new Size(364, 214);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

            this.spinner = new LoadingSpinner
            {
                Size = new Size(72, 72),
                SpinnerColor = UiTheme.Spinner
            };

            this.waitingTextLabel = new Label
            {
                AutoSize = false,
                Text = waitingText,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(UiTheme.FontFamilyName, 12F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = UiTheme.TextPrimary,
                BackColor = Color.Transparent,
                RightToLeft = RightToLeft.Yes
            };

            this.Controls.Add(this.spinner);
            this.Controls.Add(this.waitingTextLabel);
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// متنِ پنجره‌ی انتظار (مثلاً هنگام ذخیره‌سازی، ViewModel متنِ متفاوتی می‌فرستد). 
        /// </summary>
        public string WaitingText
        {
            get => this.waitingTextLabel.Text;
            set => this.waitingTextLabel.Text = value;
        }


        /// <summary>
        /// بستنِ برنامه‌ایِ پنجره، پس از اتمام منطق .
        /// (CloseReason در بستنِ برنامه‌ای همیشه قابل اتکا نیست؛ بنابراین یک پرچم صریح داریم .)
        /// </summary>
        public void CloseFromOwner()
        {
            this.isCloseRequestedByOwner = true;
            this.Close();
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            /// در زمان اجرای سازنده‌ی پایه‌ی Form، Layout ممکن است قبل از ساخته شدن فیلدها
            /// اجرا شود (مثلاً هنگام set کردن RightToLeft)؛ پس باید محافظت کنیم .
            if (this.spinner == null || this.waitingTextLabel == null)
                return;

            const int horizontalPadding = 24;

            this.spinner.Location = new Point(
                (this.ClientSize.Width - this.spinner.Width) / 2,
                34);

            this.waitingTextLabel.SetBounds(
                horizontalPadding,
                118,
                Math.Max(0, this.ClientSize.Width - (horizontalPadding * 2)),
                64);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            /// چون FormBorderStyle=None است، یک حاشیه‌ی نازک برای جدا شدن از پس‌زمینه رسم می‌کنیم .
            using Pen borderPen = new(UiTheme.BorderStrong, 1F);
            e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);

            /// یک نوارِ نازکِ رنگی در بالای پنجره، برای هم‌خانواده بودن با رنگ تأکیدِ برنامه .
            using SolidBrush accentBrush = new(UiTheme.Accent);
            e.Graphics.FillRectangle(accentBrush, 1, 1, this.Width - 2, 3);
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            /// کاربر نباید پنجره‌ی انتظار را با Alt+F4 ببندد؛
            /// فقط خودِ WaitingDialog (پس از اتمام منطق) آن را می‌بندد .
            if (this.isCloseRequestedByOwner == false && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

    }
}
