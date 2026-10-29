namespace View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class
{
    /// <summary>
    /// FlowLayoutPanel با رسمِ دوگانه (Double Buffered) .
    ///
    /// لیست خبر شاملِ ده‌ها/صدها کنترلِ آیتم است و هنگام اسکرول، رسمِ پیش‌فرض
    /// می‌تواند چشمک بزند . FlowLayoutPanel پراپرتی عمومیِ DoubleBuffered ندارد،
    /// بنابراین یک زیرکلاسِ کوچک می‌سازیم و فقط همان تنظیم را انجام می‌دهیم .
    /// </summary>
    public sealed class BufferedFlowLayoutPanel : FlowLayoutPanel
    {
        public BufferedFlowLayoutPanel()
        {
            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint, true);

            this.UpdateStyles();
        }
    }
}
