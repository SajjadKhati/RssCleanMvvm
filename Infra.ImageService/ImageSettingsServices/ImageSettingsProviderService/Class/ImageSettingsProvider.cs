using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageInfoToImageSettingsConverterService.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;

namespace Infra.ImageService.ImageSettingsServices.ImageSettingsProviderService.Class
{
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
            if(this.imageSettingsCache.ContainsKey(rssItemId) == true)
                return this.imageSettingsCache.Get(rssItemId);
            return null;
        }


        public ImageSettings<ImageInfo> GetDefault(byte[,,] image)
        {
            ImageInfo imageInfo = this.CreateDefaultImageInfo(image);
            ImageSettingsBase imageSettingsBase = this.CreateDefaultSettingsBase();
            return new ImageSettings<ImageInfo>(imageSettingsBase, imageInfo);
        }


        public ImageSettings<ImageInfo> GetOrDefault(Uri rssItemId, byte[,,] image)
        {
            ImageSettings<ImageInfo> imageSettings = this.Get(rssItemId);
            if (imageSettings == null)
            {
                imageSettings = this.GetDefault(image);
            }
            return imageSettings;
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
