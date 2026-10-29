using System.Drawing.Drawing2D;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class
{
    /// <summary>
    /// اسپینرِ چرخان پنجره‌ی انتظار (معادلِ انیمیشنِ Ellipse در WaitingWindow نسخه‌ی WPF).
    ///
    /// WPF با Storyboard یک کمان را می‌چرخاند؛ در WinForms با یک Timer و رسمِ یک کمان
    /// این کار انجام می‌شود . Timer فقط وقتی کنترل دیده می‌شود روشن است تا CPU مصرف نکند .
    /// </summary>
    public sealed class LoadingSpinner : Control
    {
        private readonly System.Windows.Forms.Timer timer;

        private float angle;


        ///////////////////////////////////////////////////////////////////////


        public LoadingSpinner()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Size = new Size(72, 72);

            this.timer = new System.Windows.Forms.Timer { Interval = 30 };
            this.timer.Tick += this.Timer_Tick;
        }


        ///////////////////////////////////////////////////////////////////////


        public Color SpinnerColor { get; set; } = UiTheme.Spinner;

        public float StrokeWidth { get; set; } = 5F;


        ///////////////////////////////////////////////////////////////////////


        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int diameter = Math.Min(this.ClientSize.Width, this.ClientSize.Height) - (int)Math.Ceiling(this.StrokeWidth) - 4;

            if (diameter <= 0)
                return;

            RectangleF arcBounds = new(
                (this.ClientSize.Width - diameter) / 2F,
                (this.ClientSize.Height - diameter) / 2F,
                diameter,
                diameter);

            using Pen pen = new(this.SpinnerColor, this.StrokeWidth)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            /// یک کمانِ ۲۷۰ درجه (شبیه اسپینرِ WPF) که با هر Tick، زاویه‌اش جلو می‌رود .
            pevent.Graphics.DrawArc(pen, arcBounds, this.angle, 270F);
        }


        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.UpdateTimerState();
        }


        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.UpdateTimerState();
        }


        protected override void OnHandleDestroyed(EventArgs e)
        {
            this.timer.Stop();
            base.OnHandleDestroyed(e);
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.angle = (this.angle + 12F) % 360F;
            this.Invalidate();
        }


        private void UpdateTimerState()
        {
            if (this.Visible == true && this.IsHandleCreated == true && this.DesignMode == false)
                this.timer.Start();
            else
                this.timer.Stop();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                this.timer.Tick -= this.Timer_Tick;
                this.timer.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
