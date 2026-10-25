namespace CompositionRoot.WIndowsFormApp.ViewModules
{
    /// <summary>
    /// پنجره‌ی انتظار ، معادل WaitingWindow در نسخه‌ی WPF .
    /// </summary>
    internal sealed class WaitingForm : Form
    {

        public WaitingForm()
        {
            this.Text = "در حال بارگذاری";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Size = new Size(360, 160);
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;

            Label messageLabel = new Label
            {
                Text = "در حال بارگذاری...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                BackColor = Color.Transparent
            };

            ProgressBar progressBar = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 18,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Margin = new Padding(20)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(24, 28, 24, 24),
                BackColor = Color.White
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layout.Controls.Add(messageLabel, 0, 0);
            layout.Controls.Add(progressBar, 0, 1);

            this.Controls.Add(layout);
        }


        ///////////////////////////////////////////////////////////////////////


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            /// یک حاشیه‌ی ساده ، چون FormBorderStyle برابر None است .
            using Pen borderPen = new Pen(Color.FromArgb(60, 120, 200), 2F);
            e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
        }

    }
}
