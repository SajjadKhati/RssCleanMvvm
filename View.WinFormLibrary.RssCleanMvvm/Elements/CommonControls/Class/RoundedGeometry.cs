using System.Drawing.Drawing2D;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.CommonControls.Class
{
    /// <summary>
    /// ساخت مسیرِ مستطیلِ گوشه‌گرد برای رسم دکمه‌ها و کنترل‌های سفارشی .
    /// (WinForms برخلاف WPF، CornerRadius توکار ندارد .)
    /// </summary>
    internal static class RoundedGeometry
    {
        public static GraphicsPath Create(Rectangle bounds, int cornerRadius)
        {
            GraphicsPath path = new();

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path;

            int diameter = Math.Min(cornerRadius * 2, Math.Min(bounds.Width, bounds.Height));

            if (diameter <= 1)
            {
                path.AddRectangle(bounds);
                return path;
            }

            Rectangle arcBounds = new(bounds.X, bounds.Y, diameter, diameter);

            path.AddArc(arcBounds, 180F, 90F);

            arcBounds.X = bounds.Right - diameter;
            path.AddArc(arcBounds, 270F, 90F);

            arcBounds.Y = bounds.Bottom - diameter;
            path.AddArc(arcBounds, 0F, 90F);

            arcBounds.X = bounds.X;
            path.AddArc(arcBounds, 90F, 90F);

            path.CloseFigure();
            return path;
        }
    }
}
