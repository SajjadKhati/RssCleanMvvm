using System.Drawing.Drawing2D;
using View.WinFormLibrary.RssCleanMvvm.AppResource.LanguageResource.Class;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class
{
    /// <summary>
    /// دکمه‌ی دو حالته (معادلِ ToggleButton در WPF) برای «حالت ویرایش / حالت خواندن» .
    ///
    /// چرا CheckBox با Appearance=Button استفاده نشد؟
    /// چون ظاهرِ آن کاملاً توسط سیستم رسم می‌شود و گوشه‌ی گرد/رنگ دلخواه را نمی‌پذیرد .
    /// این کنترل، همان قرارداد را (یک پراپرتی Checked + رویداد CheckedChanged) با رسم سفارشی
    /// ارائه می‌دهد تا در View بتوان آن را مثل یک ToggleButton بایند کرد .
    /// </summary>
    public sealed class RoundedToggleButton : Control
    {
        private bool isChecked;

        private bool isHovered;

        private bool isPressed;


        ///////////////////////////////////////////////////////////////////////


        public RoundedToggleButton()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.Selectable |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.ForeColor = UiTheme.TextPrimary;
            this.Font = UiTheme.ButtonFont;
            this.Height = UiTheme.ButtonHeight;
            this.Cursor = Cursors.Hand;
            this.TabStop = true;

            this.AccessibleRole = AccessibleRole.PushButton;
        }


        ///////////////////////////////////////////////////////////////////////


        public bool Checked
        {
            get => this.isChecked;
            set
            {
                if (this.isChecked == value)
                    return;

                this.isChecked = value;
                this.Invalidate();
                this.OnCheckedChanged(EventArgs.Empty);
            }
        }

        public event EventHandler? CheckedChanged;

        public int CornerRadius { get; set; } = UiTheme.CornerRadius;

        public Color CheckedBorderColor { get; set; } = UiTheme.ToggleCheckedBorder;

        public Color CheckedFillColor { get; set; } = UiTheme.ToggleCheckedBackground;

        public Color CheckedForeColor { get; set; } = UiTheme.ToggleCheckedText;

        public Color BorderColor { get; set; } = UiTheme.Accent;

        public Color HoverBorderColor { get; set; } = UiTheme.AccentHover;

        public Color FillColor { get; set; } = UiTheme.Surface;


        ///////////////////////////////////////////////////////////////////////


        private void OnCheckedChanged(EventArgs e)
        {
            this.CheckedChanged?.Invoke(this, e);
        }


        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bounds = new(0, 0, this.Width - 1, this.Height - 1);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            Color fillColor;
            Color borderColor;
            Color textColor;

            if (this.Enabled == false)
            {
                fillColor = UiTheme.DisabledBackground;
                borderColor = UiTheme.DisabledBorder;
                textColor = UiTheme.DisabledText;
            }
            else if (this.isChecked == true)
            {
                fillColor = this.CheckedFillColor;
                borderColor = this.CheckedBorderColor;
                textColor = this.CheckedForeColor;
            }
            else
            {
                fillColor = this.FillColor;
                borderColor = this.isPressed == true ? UiTheme.AccentPressed
                    : this.isHovered == true ? this.HoverBorderColor
                    : this.BorderColor;
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
                /// مثل ToggleButton در WPF، با کلیک فوکوس می‌گیرد تا TextBoxِ در حال ویرایش
                /// رویداد Leave را اجرا کند و متنِ ویرایش‌شده از دست نرود .
                this.Focus();

                this.isPressed = true;
                this.Invalidate();
            }
            base.OnMouseDown(mevent);
        }


        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            bool shouldToggle = this.isPressed == true &&
                                mevent.Button == MouseButtons.Left &&
                                this.ClientRectangle.Contains(mevent.Location) == true;

            this.isPressed = false;
            this.Invalidate();

            if (shouldToggle == true)
            {
                this.Checked = this.Checked == false;
                base.OnClick(EventArgs.Empty);
            }

            base.OnMouseUp(mevent);
        }


        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space || kevent.KeyCode == Keys.Enter)
            {
                this.Checked = this.Checked == false;
                base.OnClick(EventArgs.Empty);
                kevent.Handled = true;
            }

            base.OnKeyDown(kevent);
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


        protected override void OnTextChanged(EventArgs e)
        {
            this.AccessibleName = string.IsNullOrWhiteSpace(this.Text) == true
                ? PersianLanguageResource.EditMode
                : this.Text;
            this.Invalidate();
            base.OnTextChanged(e);
        }

    }
}
