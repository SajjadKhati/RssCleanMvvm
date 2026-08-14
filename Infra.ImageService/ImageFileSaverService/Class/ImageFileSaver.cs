using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Core.ImageServices.ImageCreatorService.Class;
using ImageMagick;
using Infra.ImageService.MappingService.Class;

namespace Infra.ImageService.ImageFileSaverService.Class
{
    public class ImageFileSaver : IImageFileSaver
    {

        private ImageUtility imageUtility;

        private ImageFileSettingsMemberMapper imageSaverSettingsMemberMapper;


        /////////////////////////////////////////////////////////
        

        public ImageFileSaver(ImageUtility imageUtility, ImageFileSettingsMemberMapper imageSaverSettingsMemberMapper)
        {
            this.imageUtility = imageUtility;
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
        public void Save8BitImageToFile(ImageFileSaveSettings imageFileSaveSettings)
        {
            if (imageFileSaveSettings == null || imageFileSaveSettings.ImageFileSettings == null ||
                imageFileSaveSettings.ImageFileSettings.ImageDetails == null || imageFileSaveSettings.ImageFileSettings.ImageDetails.Data == null) 
                throw new ArgumentNullException(nameof(imageFileSaveSettings.ImageFileSettings.ImageDetails.Data));
            if (string.IsNullOrEmpty(imageFileSaveSettings.FullFilePath)) 
                throw new ArgumentException("File path is required.", nameof(imageFileSaveSettings.FullFilePath));

            var imageData = imageFileSaveSettings.ImageFileSettings.ImageDetails.Data;
            // ۱. استخراج ابعاد و اطلاعات از آرایه
            int height = imageData.GetLength(0);
            int width = imageData.GetLength(1);
            int channels = imageData.GetLength(2);

            // ۳. تخت کردن داده‌ها
            byte[] flatData = this.imageUtility.FlattenImageData(imageData, height, width, channels);

            // ۴. ایجاد MagickImage و اعمال تنظیمات
            using var magickImage = this.CreateMagickImage(flatData, width, height, imageFileSaveSettings.ImageFileSettings);

            // ۵. ذخیره
            magickImage.Write(imageFileSaveSettings.FullFilePath);
        }


        /// <summary> ساخت MagickImage بر اساس داده‌ها و تنظیمات </summary>
        private MagickImage CreateMagickImage<TImageInfo>(byte[] flatData, int width, int height, ImageSettings<TImageInfo> imageSettings)
            where TImageInfo : ImageInfo
        {
            var pixelSettings = new PixelReadSettings((uint)width, (uint)height,
                this.imageSaverSettingsMemberMapper.ToMagickStorageType(imageSettings.StorageType), 
                this.imageSaverSettingsMemberMapper.ToMagickPixelMapping(imageSettings.Mapping));

            var magickImage = new MagickImage(flatData, pixelSettings);
            var imageData = imageSettings.ImageDetails;
            magickImage.Depth = (uint)imageData.ColorDepth;
            magickImage.ColorSpace = this.imageSaverSettingsMemberMapper.ImageInfoMemberMapper.ToMagicColorSpace(imageData.ColorSpace);
            magickImage.Alpha(this.imageSaverSettingsMemberMapper.ToMagickAlphaOption(imageSettings.HaveAlphaChannel));
            magickImage.Format = this.imageSaverSettingsMemberMapper.ToMagickMagickFormat(imageSettings.Format);

            return magickImage;
        }

    }
}
