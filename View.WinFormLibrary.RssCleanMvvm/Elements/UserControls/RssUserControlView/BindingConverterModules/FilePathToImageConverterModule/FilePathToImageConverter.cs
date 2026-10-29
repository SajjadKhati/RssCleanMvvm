using System.Drawing.Drawing2D;
using View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class;

namespace View.WinFormLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView.BindingConverterModules.FilePathToImageConverterModule
{
    /// <summary>
    /// معادلِ <c>FilePathToBitmapImageConverter</c> در نسخه‌ی WPF .
    ///
    /// در WPF یک <c>IValueConverter</c> بود که مسیر فایل را به BitmapImage تبدیل می‌کرد .
    /// در WinForms فایل مستقیم به <c>Image</c> تبدیل می‌شود، ولی همان دو نکته‌ی مهم حفظ شده است :
    ///
    ///   ۱) فایل با FileShare.ReadWrite باز می‌شود تا عملیاتِ «تبدیل به سیاه و سفید» که همان
    ///      فایل را بازنویسی می‌کند، با خطای قفلِ فایل مواجه نشود .
    ///
    ///   ۲) تصویر به‌اندازه‌ی نهاییِ کادر (۱۹۲×۱۰۸) کوچک و از مرکز برش داده می‌شود .
    ///      این معادلِ <c>Stretch="UniformToFill"</c> در WPF است : کل کادر پر می‌شود،
    ///      نسبتِ ابعاد حفظ می‌شود و بخش اضافی از وسط بریده می‌شود .
    ///
    /// نکته‌ی WinForms: برخلاف WPF که BitmapImage را با StreamSource می‌سازد و فایل را
    /// قفل نمی‌کند، اینجا تصویر را کامل در یک Bitmap جدید کپی می‌کنیم و stream اصلی را
    /// می‌بندیم؛ پس فایل روی دیسک آزاد می‌ماند (مهم برای جایگزینی/حذف فایل) .
    /// </summary>
    public static class FilePathToImageConverter
    {
        public static Image? Convert(string? imageFullFilePath, int targetWidth, int targetHeight)
        {
            if (targetWidth <= 0 || targetHeight <= 0)
                return null;

            if (string.IsNullOrWhiteSpace(imageFullFilePath) == true)
                return null;

            if (File.Exists(imageFullFilePath) == false)
                return null;

            try
            {
                using FileStream fileStream = new(imageFullFilePath, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete);

                /// validateImageData=false فقط برای سرعت است؛ اگر داده معتبر نباشد، استثنا می‌گیریم .
                using Image sourceImage = Image.FromStream(fileStream, useEmbeddedColorManagement: false,
                    validateImageData: false);

                if (sourceImage.Width <= 0 || sourceImage.Height <= 0)
                    return null;

                Bitmap targetBitmap = new(targetWidth, targetHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                using (Graphics graphics = Graphics.FromImage(targetBitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                    graphics.Clear(UiTheme.ImagePlaceholder);

                    /// UniformToFill : بزرگ‌ترین نسبت را انتخاب می‌کنیم تا کل کادر پر شود .
                    double scale = Math.Max((double)targetWidth / sourceImage.Width,
                        (double)targetHeight / sourceImage.Height);

                    int sourceWidth = Math.Max(1, (int)Math.Round(targetWidth / scale));
                    int sourceHeight = Math.Max(1, (int)Math.Round(targetHeight / scale));

                    int sourceX = Math.Max(0, (sourceImage.Width - sourceWidth) / 2);
                    int sourceY = Math.Max(0, (sourceImage.Height - sourceHeight) / 2);

                    graphics.DrawImage(sourceImage,
                        new Rectangle(0, 0, targetWidth, targetHeight),
                        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel);
                }

                return targetBitmap;
            }
            catch (Exception)
            {
                /// تصویرِ خراب/حذف‌شده نباید کل لیست را از کار بیندازد؛ فقط بدون تصویر نمایش داده می‌شود .
                return null;
            }
        }
    }
}
