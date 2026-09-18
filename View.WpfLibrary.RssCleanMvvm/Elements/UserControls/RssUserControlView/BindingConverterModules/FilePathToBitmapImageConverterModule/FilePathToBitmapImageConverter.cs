using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace View.WpfLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView.BindingConverterModules.FilePathToBitmapImageConverterModule
{
    public class FilePathToBitmapImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filePath = this.FilePathValidate(value);
            if (filePath == null)
                return null;
            double imageElementWidth = 192;
            if (parameter is Double paramImageElementWidth && paramImageElementWidth > 0)
                imageElementWidth = paramImageElementWidth;

            BitmapImage bitmapImage = this.CreateBitmapImageByFilePath(filePath, imageElementWidth);
            return bitmapImage;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        ///////////////////////////////////////////////////////////


        private string FilePathValidate(object pathValidate)
        {
            /// توضیح نکته ی معماری درباره ی File.Exists :
            ///
            /// از نظر معماری تمیز (Clean Architecture):
            ///   اعتبارسنجی وجود فایل، وظیفهٔ لایهٔ Application یا ViewModel است،
            ///   نه وظیفهٔ یک IValueConverter در لایهٔ Presentation.
            ///   منطقاً باید ViewModel تضمین کند که ImageFullFilePath همیشه معتبر است
            ///   و اگر نبود، اصلاً Binding اجرا نشود.
            ///
            /// اما در عمل (Defensive Programming):
            ///   Converter نمی‌داند که چه کسی آن را صدا زده و آیا مسیر معتبر است یا نه.
            ///   اگر File.Exists را حذف کنیم و فایل وجود نداشته باشد:
            ///     → FileStream با FileNotFoundException کرش می‌کند.
            ///     → WPF این Exception را در Bindings به‌درستی مدیریت نمی‌کند.
            ///     → ممکن است برنامه با پیام خطای نامفهوم بسته شود.
            ///
            /// چرا تصمیم گرفتیم اینجا نگه داریم:
            ///   ۱) هزینه‌اش ناچیز است (یک چک ساده روی فایل سیستم).
            ///   ۲) از کرش جلوگیری می‌کند.
            ///   ۳) یک لایهٔ دفاعی (Defense in Depth) در برابر خطای احتمالی VM است.
            ///
            /// ⚠️ این یک «انحراف آگاهانه از معماری خالص» است که با آگاهی از هزینه‌اش
            ///    پذیرفته شده. اگر روزی VM تضمین کرد که مسیر همیشه معتبر است،
            ///    می‌توان این چک را حذف کرد.
            if (pathValidate is not string filePath || string.IsNullOrWhiteSpace(filePath) == true || File.Exists(filePath) == false)
                return null;
            return filePath;
        }


        private BitmapImage CreateBitmapImageByFilePath(string filePath, double decodePixelWidth)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            /// توضیحات CacheOption = OnLoad :
            /// ---------------------
            /// به WPF می‌گوید: «کل تصویر را همین حالا در حافظه بارگذاری کن و 
            /// فایل روی دیسک را بلافاصله آزاد کن.»
            ///
            /// چرا OnLoad و نه OnDemand (پیش‌فرض)؟
            ///   - با OnDemand، WPF فایل را تا زمانی که تصویر استفاده می‌شود باز نگه می‌دارد.
            ///   - این باعث می‌شود نتوانیم فایل را بعداً حذف یا جایگزین کنیم 
            ///     (خطای «file in use»).
            ///   - در برنامهٔ ما، فایل تصویر موقت ممکن است بعداً حذف یا 
            ///     بازنویسی (تبدیل به سیاه و سفید) شود → OnLoad ضروری است.
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            /// توضیحات DecodePixelWidth :
            /// ----------------
            /// تعیین می‌کند تصویر با «حداکثر این عرض» در حافظه دیکد شود.
            ///
            /// چرا مهم است؟
            ///   تصویر اصلی ممکن است 800×600 یا 2000×1500 باشد. اگر آن را با ابعاد کامل
            ///   دیکد کنیم، حافظهٔ زیادی مصرف می‌شود (عرض × ارتفاع × 4 بایت).
            ///   با DecodePixelWidth = 200، فقط 200×150 دیکد می‌شود → ۱۶ برابر کمتر.
            ///
            /// چرا فقط یکی از DecodePixelWidth/DecodePixelHeight کافی است؟
            ///   چون WPF خودش بعد دیگر را «متناسب با نسبت ابعاد اصلی» محاسبه می‌کند.
            ///   اگر هر دو را بدهید و نسبتشان با تصویر اصلی نخواند،
            ///   WPF تصویر را می‌کشد و اعوجاج ایجاد می‌کند.
            ///   پس فقط یکی را می‌دهیم تا نسبت ابعاد حفظ شود.
            bitmapImage.DecodePixelWidth = ((int)decodePixelWidth * 2);
            /// توضیحات FileStream :
            ///
            /// path:
            ///   مسیر فایل تصویر روی دیسک.
            ///
            /// FileMode.Open:
            ///   فایل موجود را باز کن (نساز).
            ///   اگر وجود نداشته باشد → FileNotFoundException.
            ///
            /// FileAccess.Read:
            ///   فقط برای خواندن باز کن (نه نوشتن).
            ///   جلوگیری از تغییر ناخواستهٔ فایل.
            ///
            /// FileShare.Read:
            ///   اجازه بده بقیه هم بتوانند فایل را بخوانند.
            ///   ولی نوشتن ممنوع (بدون FileShare.Write).
            ///   چرا؟ چون ممکن است همان لحظه سرویس دیگری (مثلاً حذف فایل موقت)
            ///   بخواهد بخواند. اجازه دادن به خواندن مشکلی ندارد.
            ///
            /// using:
            ///   بعد از اتمام کار، stream به‌طور خودکار بسته می‌شود.
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            bitmapImage.StreamSource = fileStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

    }
}
