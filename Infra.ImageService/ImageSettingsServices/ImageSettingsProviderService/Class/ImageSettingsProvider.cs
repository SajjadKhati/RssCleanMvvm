using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageInfoToImageSettingsConverterService.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;

namespace Infra.ImageService.ImageSettingsServices.ImageSettingsProviderService.Class
{
    /// <summary>
    /// ارائه‌دهنده‌ی ImageSettings — با cache داخلی برای ذخیره‌ی تنظیمات هر RssItem.
    /// 
    /// ⚠️ چرا این کلاس قفل ندارد؟
    /// 
    /// اگر چه این کلاس یک فیلد داده‌ای مشترک دارد (imageSettingsCache)، ولی قفل
    /// لازم نیست — به سه دلیل:
    /// 
    /// ۱. تمام عملیات روی cache، تک‌عملیات اتمی هستند (Set, TryGetValue). 
    ///    ConcurrentDictionary خودش اتمی بودنشان را تضمین می‌کند.
    /// 
    /// ۲. هیچ invariant بین چند عملیات وجود ندارد. هر متد فقط یک عملیات 
    ///    اتمی روی cache انجام می‌دهد.
    /// 
    /// ۳. حتی در GetOrDefault که یک check-then-act است (Get، سپس در صورت 
    ///    null بودن GetDefault)، هر دو جواب معتبرند — هم نسخه‌ی cache و هم 
    ///    default settings. بنابراین قفل، هیچ چیز اضافه‌ای را تضمین نمی‌کند.
    ///    (برای توضیح بیشتر، توضیحات متد GetOrDefault را ببینید.)
    /// </summary>
    public class ImageSettingsProvider : IImageSettingsWriter, IImageSettingsReader
    {

        private ICache<Uri, ImageSettings<ImageInfo>> imageSettingsCache;

        private IImageInfoToImageSettingsConverter imageSettingsConverter;


        ///////////////////////////////////////////////////////////////////////////////////


        private readonly int defaultColorDepth = 8;
        private readonly ColorSpace defaultColorSpace = ColorSpace.sRGB;
        private readonly PixelChannel[] defaultChannelsSequence = { PixelChannel.Alpha, PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue };


        ///////////////////////////////////////////////////////////////////////////////////


        public ImageSettingsProvider(ICache<Uri, ImageSettings<ImageInfo>> imageSettingsCache,
            IImageInfoToImageSettingsConverter imageSettingsConverter)
        {
            this.imageSettingsCache = imageSettingsCache;
            this.imageSettingsConverter = imageSettingsConverter;
        }


        ///////////////////////////////////////////////////////////////////////////////////


        public void Store(Uri rssItemId, ImageInfo imageInfo)
        {
            /// شی imageInfoSnapshot ، کپی شد ممکنه کاربر ، شی ای که از imageInfo داده ، در واقع ImageData بوده باشه که چون داده ی تصویر را هم داره ، حجم زیادی ببره .
            ImageInfo imageInfoSnapshot = new ImageInfo(imageInfo);
            ImageSettingsBase imageSettingsBase = this.imageSettingsConverter.Convert(imageInfoSnapshot);
            ImageSettings<ImageInfo> imageSettings = new ImageSettings<ImageInfo>(imageSettingsBase, imageInfoSnapshot);
            this.imageSettingsCache.Set(rssItemId, imageSettings);
        }


        public ImageSettings<ImageInfo> Get(Uri rssItemId)
        {
            return (this.imageSettingsCache.TryGetValue(rssItemId, out ImageSettings<ImageInfo> imageSetting) == true)? imageSetting : null;
        }


        /// <summary>
        /// گرفتن تنظیمات تصویر برای یک RssItem — از cache، و اگر نبود، default settings.
        /// 
        /// ⚠️ چرا این متد قفل ندارد؟ (check-then-act)
        /// 
        /// این متد یک الگوی check-then-act دارد: ابتدا Get از cache، و اگر null 
        /// بود، GetDefault. اگر بین این دو، نخ دیگری Store کند، ممکن است default 
        /// برگردانده شود در حالی که نسخه‌ی جدید در cache هست.
        /// 
        /// ولی این race بدون ضرر است — به سه دلیل:
        /// 
        /// ۱. هر دو جواب معتبرند. default settings هم یک تنظیمات کامل و قابل 
        ///    استفاده است (فقط کمی ساده‌تر از تنظیمات ذخیره‌شده در cache).
        /// 
        /// ۲. داده از دست نمی‌رود. نسخه‌ی Store شده در cache باقی می‌ماند و 
        ///    دفعه‌ی بعد (در فراخوانی مجدد) استفاده می‌شود.
        /// 
        /// ۳. فاصله‌ی زمانی بین Get و GetDefault بسیار کوتاه است (چند نانوثانیه) 
        ///    — چون GetDefault فقط از image می‌سازد و به cache دست نمی‌زند. 
        ///    احتمال اینکه نخ دیگری دقیقاً در این فاصله Store کند، بسیار کم است.
        /// 
        /// قفل، در این حالت فقط سربار است — چون خروجی نهایی (default) هم معتبر 
        /// است، و atomicity چیزی به درستی این متد اضافه نمی‌کند.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public ImageSettings<ImageInfo> GetDefault(byte[,,] image)
        {
            ImageInfo imageInfo = this.CreateDefaultImageInfo(image);
            ImageSettingsBase imageSettingsBase = this.CreateDefaultSettingsBase();
            return new ImageSettings<ImageInfo>(imageSettingsBase, imageInfo);
        }


        public ImageSettings<ImageInfo> GetOrDefault(Uri rssItemId, byte[,,] image)
        {
            ImageSettings<ImageInfo> imageSettings = this.Get(rssItemId);
            return (imageSettings == null) ? this.GetDefault(image) : imageSettings;
        }


        ///////////////////////////////////////////////////////////////////////////////////


        private ImageInfo CreateDefaultImageInfo(byte[,,] image)
        {
            return new ImageInfo
            {
                Height = image.GetLength(0),
                Width = image.GetLength(1),
                ChannelsCount = image.GetLength(2),
                ColorDepth = this.defaultColorDepth,
                ColorSpace = this.defaultColorSpace,
                ChannelsSequence = this.defaultChannelsSequence
            };
        }


        private ImageSettingsBase CreateDefaultSettingsBase()
        {
            return new ImageSettingsBase
            {
                StorageType = StorageType.Byte,
                Mapping = PixelMapping.ARGB,
                HaveAlphaChannel = AlphaOption.On,
                Format = MagickFormat.Png32
            };
        }

    }
}
