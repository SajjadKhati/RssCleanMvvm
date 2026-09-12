using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageInfoToImageSettingsConverterService.Interface;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;

namespace Infra.ImageService.ImageSettingsServices.ImageInfoToImageSettingsConverterService.Class
{
    public class ImageInfoToImageSettingsConverter : IImageInfoToImageSettingsConverter
    {

        private const int BitsPerByte = 8;
        private const int BitsPerShort = 16;
        private const int BitsPerInt32 = 32;
        private const int BitsPerInt64 = 64;

        private const int RgbChannelCount = 3;
        private const int RgbaChannelCount = 4;

        private readonly PixelChannel[] rgbOrder = { PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue };

        private readonly PixelChannel[] bgrOrder = { PixelChannel.Blue, PixelChannel.Green, PixelChannel.Red };

        private readonly PixelChannel[] rgbaOrder = { PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue, PixelChannel.Alpha };

        private readonly PixelChannel[] bgraOrder = { PixelChannel.Blue, PixelChannel.Green, PixelChannel.Red, PixelChannel.Alpha };

        private readonly PixelChannel[] argbOrder = { PixelChannel.Alpha, PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue };

        private readonly PixelChannel[] abgrOrder = { PixelChannel.Alpha, PixelChannel.Blue, PixelChannel.Green, PixelChannel.Red };


        /////////////////////////////////////////////////////////////////////////


        public ImageSettingsBase Convert(ImageInfo imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException(nameof(imageInfo));

            if (imageInfo.ChannelsSequence == null)
                throw new NotSupportedException($"{nameof(imageInfo.ChannelsSequence)} نمی‌تواند null یا خالی باشد .");

            StorageType storageType = this.ConvertToStorageType(imageInfo.ColorDepth);
            bool hasAlpha = this.HasAlphaChannel(imageInfo.ChannelsSequence);
            PixelMapping mapping = this.ConvertToPixelMapping(imageInfo.ChannelsSequence);
            MagickFormat format = this.ConvertToFormat(hasAlpha);

            return new ImageSettingsBase
            {
                StorageType = storageType,
                Mapping = mapping,
                HaveAlphaChannel = hasAlpha ? AlphaOption.On : AlphaOption.Off,
                Format = format
            };
        }


        /////////////////////////////////////////////////////////////////////////


        private StorageType ConvertToStorageType(int colorDepth)
        {
            return colorDepth switch
            {
                BitsPerByte => StorageType.Byte,
                BitsPerShort => StorageType.Short,
                BitsPerInt32 => StorageType.Int32,
                BitsPerInt64 => StorageType.Int64,
                _ => throw new NotSupportedException($"عمق رنگ پشتیبانی نمی‌شود: {colorDepth}")
            };
        }


        private bool HasAlphaChannel(IReadOnlyList<PixelChannel> channels)
        {
            return channels.Contains(PixelChannel.Alpha);
        }


        private PixelMapping ConvertToPixelMapping(IReadOnlyList<PixelChannel> channels)
        {
            // تصویر تک‌کاناله (Gray) پشتیبانی نمی‌شود
            if (channels.Count == 1)
                throw new NotSupportedException("تصاویر تک‌کاناله (Gray) پشتیبانی نمی‌شوند.");

            // تصاویر Indexed پشتیبانی نمی‌شوند
            if (channels.Contains(PixelChannel.Index))
                throw new NotSupportedException("تصاویر Indexed پشتیبانی نمی‌شوند.");

            // فقط ۳ یا ۴ کانال مجاز است
            if (channels.Count != RgbChannelCount && channels.Count != RgbaChannelCount)
                throw new NotSupportedException($"فقط ۳ یا ۴ کانال مجاز است  .\nتعداد کانال‌های این تصویر که : {channels.Count} تا هست ، پشتیبانی نمیشود .");

            if (channels.Count == 3)
            {
                if (channels.SequenceEqual(this.rgbOrder) == true)
                    return PixelMapping.RGB;
                if (channels.SequenceEqual(this.bgrOrder) == true)
                    return PixelMapping.BGR;
            }
            else if (channels.Count == 4)
            {
                if (channels.SequenceEqual(this.rgbaOrder) == true)
                    return PixelMapping.RGBA;
                if (channels.SequenceEqual(this.bgraOrder) == true)
                    return PixelMapping.BGRA;
                if (channels.SequenceEqual(this.argbOrder) == true)
                    return PixelMapping.ARGB;
                if (channels.SequenceEqual(this.abgrOrder) == true)
                    return PixelMapping.ABGR;
            }
            else
            {
                throw new NotSupportedException($"تعداد کانال‌های تصویر جاری ، پشتیبانی ‌نمیشود .\nتعدادِ کانال های تصویر جاری که پشتیبانی نمیشود ، {channels.Count} تا هست .");
            }

            throw new NotSupportedException(
                $"ترتیب کانال‌های تصویر جاری ، پشتیبانی ‌نمیشود .\nترتیبِ کانال های تصویر جاری که پشتیبانی نمیشود ، با این ترتیب هست : [{string.Join(", ", channels)}]");
        }


        private MagickFormat ConvertToFormat(bool hasAlpha)
        {
            return hasAlpha ? MagickFormat.Png32 : MagickFormat.Jpeg;
        }

    }
}
