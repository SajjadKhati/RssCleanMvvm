using System.Drawing.Drawing2D;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class
{
    /// <summary>
    /// دکمه‌ی گوشه‌گرد، معادلِ دکمه‌ی حاشیه‌دار (BorderedButton) نسخه‌ی WPF .
    ///
    /// چرا کنترل سفارشی؟
    /// WinForms دکمه‌ی استاندارد با گوشه‌ی گرد ندارد و CornerRadius در Button وجود ندارد .
    /// با بازنویسی OnPaint و یک GraphicsPath، همان ظاهر (حاشیه‌ی آبی + گوشه‌ی گرد + حالت
    /// hover/pressed/disabled) بدون هیچ کتابخانه‌ی جانبی ساخته می‌شود . این تنها
    /// «بازسازی ظاهر» است و هیچ منطقی به آن اضافه نشده .
    /// </summary>
    public class RoundedButton : Button
    {
        private bool isHovered;

        private bool isPressed;


        ///////////////////////////////////////////////////////////////////////


        public RoundedButton()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.UseVisualStyleBackColor = false;

            this.BackColor = UiTheme.Surface;
            this.ForeColor = UiTheme.TextPrimary;
            this.Font = UiTheme.ButtonFont;
            this.Height = UiTheme.ButtonHeight;
            this.Cursor = Cursors.Hand;
            this.TabStop = true;
        }


        ///////////////////////////////////////////////////////////////////////


        /// <summary>شعاع گوشه‌ها (پیش‌فرض از پالت مرکزی).</summary>
        public int CornerRadius { get; set; } = UiTheme.CornerRadius;

        public Color BorderColor { get; set; } = UiTheme.Accent;

        public Color HoverBorderColor { get; set; } = UiTheme.AccentHover;

        public Color PressedBorderColor { get; set; } = UiTheme.AccentPressed;

        public Color DisabledBorderColor { get; set; } = UiTheme.DisabledBorder;

        public Color FillColor { get; set; } = UiTheme.Surface;

        public Color HoverFillColor { get; set; } = UiTheme.AccentSoft;

        public Color PressedFillColor { get; set; } = UiTheme.AccentSoft;

        public Color DisabledFillColor { get; set; } = UiTheme.DisabledBackground;

        public Color DisabledForeColor { get; set; } = UiTheme.DisabledText;


        ///////////////////////////////////////////////////////////////////////


        protected override void OnPaint(PaintEventArgs pevent)
        {
            /// base.OnPaint را صدا نمی‌زنیم چون کل ظاهر دکمه را خودمان رسم می‌کنیم .
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bounds = new(0, 0, this.Width - 1, this.Height - 1);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color fillColor;
            Color borderColor;
            Color textColor;

            if (this.Enabled == false)
            {
                fillColor = this.DisabledFillColor;
                borderColor = this.DisabledBorderColor;
                textColor = this.DisabledForeColor;
            }
            else if (this.isPressed == true)
            {
                fillColor = this.PressedFillColor;
                borderColor = this.PressedBorderColor;
                textColor = this.ForeColor;
            }
            else if (this.isHovered == true)
            {
                fillColor = this.HoverFillColor;
                borderColor = this.HoverBorderColor;
                textColor = this.ForeColor;
            }
            else
            {
                fillColor = this.FillColor;
                borderColor = this.BorderColor;
                textColor = this.ForeColor;
            }

            using (GraphicsPath path = RoundedGeometry.Create(bounds, this.CornerRadius))
            {
                using (SolidBrush fillBrush = new(fillColor))
                    pevent.Graphics.FillPath(fillBrush, path);

                using (Pen borderPen = new(borderColor, 1.4F))
                    pevent.Graphics.DrawPath(borderPen, path);
            }

            TextFormatFlags flags = TextFormatFlags.HorizontalCenter |
                                    TextFormatFlags.VerticalCenter |
                                    TextFormatFlags.NoPrefix |
                                    TextFormatFlags.EndEllipsis;

            TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, bounds, textColor, flags);

            if (this.Focused == true && this.ShowFocusCues == true)
            {
                Rectangle focusBounds = Rectangle.Inflate(bounds, -4, -4);
                ControlPaint.DrawFocusRectangle(pevent.Graphics, focusBounds, textColor, fillColor);
            }
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            this.isHovered = true;
            this.Invalidate();
            base.OnMouseEnter(e);
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            this.isHovered = false;
            this.isPressed = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }


        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                this.isPressed = true;
                this.Invalidate();
            }
            base.OnMouseDown(mevent);
        }


        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            this.isPressed = false;
            this.Invalidate();
            base.OnMouseUp(mevent);
        }


        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space || kevent.KeyCode == Keys.Enter)
            {
                this.isPressed = true;
                this.Invalidate();
            }
            base.OnKeyDown(kevent);
        }


        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space || kevent.KeyCode == Keys.Enter)
            {
                this.isPressed = false;
                this.Invalidate();
            }
            base.OnKeyUp(kevent);
        }


        protected override void OnEnabledChanged(EventArgs e)
        {
            this.Cursor = this.Enabled == true ? Cursors.Hand : Cursors.Default;
            this.Invalidate();
            base.OnEnabledChanged(e);
        }


        protected override void OnGotFocus(EventArgs e)
        {
            this.Invalidate();
            base.OnGotFocus(e);
        }


        protected override void OnLostFocus(EventArgs e)
        {
            this.Invalidate();
            base.OnLostFocus(e);
        }

    }
}
