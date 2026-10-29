using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class
{
    /// <summary>
    /// جایگزینِ <c>TextBlock</c> در WPF .
    ///
    /// چرا Label معمولی کافی نیست؟
    ///   • Label با AutoSize=true بر اساس عرض نمی‌شکند .
    ///   • Label با AutoSize=false ارتفاعش را با متنِ چندخطی بزرگ نمی‌کند .
    ///
    /// این کنترل :
    ///   • ارتفاعِ لازم را با توجه به عرضِ واقعیِ سلولِ TableLayoutPanel محاسبه می‌کند
    ///     (<see cref="GetPreferredSize"/>)
    ///   • بنابراین ردیفِ AutoSize در جدول، به اندازه‌ی متنِ پیچیده‌شده بلند می‌شود .
    ///   • جهت راست‌به‌چپ را هنگام رسم، صریحاً رعایت می‌کند (برخلاف Label که
    ///     TextAlign اش در حالت RTL برعکس می‌شود) .
    ///
    /// اگر <see cref="Wrap"/> برابر false باشد، مثل یک TextBlock تک‌خطی با Ellipsis کار می‌کند .
    /// </summary>
    public sealed class WrappedLabel : Control
    {
        private const int FallbackWidth = 400;

        private const int VerticalPadding = 2;


        ///////////////////////////////////////////////////////////////////////


        public WrappedLabel()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.SetStyle(ControlStyles.Selectable, false);

            this.BackColor = Color.Transparent;
            this.ForeColor = UiTheme.TextPrimary;
            this.Font = UiTheme.BaseFont;
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>اگر true باشد، متن در عرضِ موجود می‌شکند و ارتفاع رشد می‌کند .</summary>
        public bool Wrap { get; set; } = true;


        ///////////////////////////////////////////////////////////////////////


        public override Size GetPreferredSize(Size proposedSize)
        {
            int availableWidth = proposedSize.Width;

            if (availableWidth <= 0 || availableWidth == int.MaxValue)
                availableWidth = this.Width > 0 ? this.Width : FallbackWidth;

            int lineHeight = this.Font.Height;

            if (this.Wrap == false)
                return new Size(availableWidth, lineHeight + (VerticalPadding * 2));

            if (string.IsNullOrEmpty(this.Text) == true)
                return new Size(availableWidth, lineHeight + VerticalPadding);

            Size measuredSize = TextRenderer.MeasureText(this.Text, this.Font,
                new Size(availableWidth, int.MaxValue), this.CreateTextFormatFlags());

            return new Size(availableWidth, Math.Max(measuredSize.Height, lineHeight) + VerticalPadding);
        }


        protected override void OnPaint(PaintEventArgs pevent)
        {
            /// base.OnPaint را صدا نمی‌زنیم چون متن را خودمان با TextRenderer می‌کشیم .
            if (string.IsNullOrEmpty(this.Text) == true)
                return;

            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
                return;

            TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle,
                this.ForeColor, this.CreateTextFormatFlags());
        }


        protected override void OnTextChanged(EventArgs e)
        {
            this.Invalidate();
            this.NotifyParentLayout();
            base.OnTextChanged(e);
        }


        protected override void OnFontChanged(EventArgs e)
        {
            this.Invalidate();
            this.NotifyParentLayout();
            base.OnFontChanged(e);
        }


        protected override void OnRightToLeftChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnRightToLeftChanged(e);
        }


        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            base.OnResize(e);
        }


        private TextFormatFlags CreateTextFormatFlags()
        {
            TextFormatFlags flags = TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

            if (this.Wrap == true)
            {
                flags |= TextFormatFlags.WordBreak;
            }
            else
            {
                flags |= TextFormatFlags.SingleLine |
                         TextFormatFlags.VerticalCenter |
                         TextFormatFlags.EndEllipsis;
            }

            /// WinForms به‌طور خودکار، جهتِ رسمِ TextRenderer را از RightToLeft کنترل نمی‌گیرد .
            if (this.RightToLeft == RightToLeft.Yes)
                flags |= TextFormatFlags.Right | TextFormatFlags.RightToLeft;

            return flags;
        }


        private void NotifyParentLayout()
        {
            /// متن که عوض شود، ارتفاعِ لازم هم عوض می‌شود؛ پس ردیفِ AutoSize جدول باید دوباره چیده شود .
            this.Parent?.PerformLayout();
        }

    }
}
