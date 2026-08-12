using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using ImageMagick;
using Infra.ImageService.MappingService.Class;

namespace Infra.ImageService.ImageFileSaverService.Class
{
    public class ImageFileSaver : IImageFileSaver
    {

        private ImageFileSettingsMemberMapper imageSaverSettingsMemberMapper;


        /////////////////////////////////////////////////////////
        

        public ImageFileSaver(ImageFileSettingsMemberMapper imageSaverSettingsMemberMapper)
        {
            this.imageSaverSettingsMemberMapper = imageSaverSettingsMemberMapper;
        }


        /////////////////////////////////////////////////////////


        /// <summary>
        /// private ImageFileSettings ApplyDefaults(ImageFileSettings? userSettings)
        /// {
        ///     return new ImageFileSettings
        ///     {
        ///         StorageType     = StorageType.Char,
        ///         Mapping         = PixelMapping.ARGB,
        ///         HaveAlphaChannel = AlphaOption.On,
        ///         Format          =  MagickFormat.Png32,
        ///         ImageInfo       =  ImageInfo
        ///         {
        ///         ColorSpace = ColorSpace.sRGB,
        ///         ColorDepth = 8
        ///     }
        ///     };
        /// }
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="fullFilePath"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Save8BitImageToFile(ImageFileSpecification imageFileSpecification)
        {
            if (imageFileSpecification == null || imageFileSpecification.ImageFileSettings == null || 
                imageFileSpecification.ImageFileSettings.ImageData == null || imageFileSpecification.ImageFileSettings.ImageData.Data == null) 
                throw new ArgumentNullException(nameof(imageFileSpecification.ImageFileSettings.ImageData.Data));
            if (string.IsNullOrEmpty(imageFileSpecification.FullFilePath)) 
                throw new ArgumentException("File path is required.", nameof(imageFileSpecification.FullFilePath));

            var imageData = imageFileSpecification.ImageFileSettings.ImageData.Data;
            // ۱. استخراج ابعاد و اطلاعات از آرایه
            int height = imageData.GetLength(0);
            int width = imageData.GetLength(1);
            int channels = imageData.GetLength(2);

            // ۳. تخت کردن داده‌ها
            byte[] flatData = FlattenImageData(imageData, height, width, channels);

            // ۴. ایجاد MagickImage و اعمال تنظیمات
            using var image = CreateMagickImage(flatData, width, height, imageFileSpecification.ImageFileSettings);

            // ۵. ذخیره
            image.Write(imageFileSpecification.FullFilePath);
        }


        /// <summary> تخت کردن آرایه سه‌بعدی به آرایه یک‌ بعدی </summary>
        private byte[] FlattenImageData(byte[,,] imageData, int height, int width, int channels)
        {
            var flat = new byte[width * height * channels];
            int index = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    for (int c = 0; c < channels; c++)
                        flat[index++] = imageData[y, x, c];
            return flat;
        }


        /// <summary> ساخت MagickImage بر اساس داده‌ها و تنظیمات </summary>
        private MagickImage CreateMagickImage(byte[] flatData, int width, int height, ImageFileSettings settings)
        {
            var pixelSettings = new PixelReadSettings((uint)width, (uint)height,
                this.imageSaverSettingsMemberMapper.ToMagickStorageType(settings.StorageType), 
                this.imageSaverSettingsMemberMapper.ToMagickPixelMapping(settings.Mapping));

            var image = new MagickImage(flatData, pixelSettings);
            var imageData = settings.ImageData;
            image.Depth = (uint)imageData.ColorDepth;
            image.ColorSpace = this.imageSaverSettingsMemberMapper.ImageInfoMemberMapper.ToMagicColorSpace(imageData.ColorSpace);
            image.Alpha(this.imageSaverSettingsMemberMapper.ToMagickAlphaOption(settings.HaveAlphaChannel));
            image.Format = this.imageSaverSettingsMemberMapper.ToMagickMagickFormat(settings.Format);

            return image;
        }

    }
}
