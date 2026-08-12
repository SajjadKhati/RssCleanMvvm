using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using System.ComponentModel;

namespace Infra.ImageService.MappingService.Class
{
    public class ImageInfoMemberMapper
    {

        public ImageMagick.ColorSpace ToMagicColorSpace(ColorSpace applicationColorSpace)
        {
            return applicationColorSpace switch
            {
                    ColorSpace.Undefined => ImageMagick.ColorSpace.Undefined,
                    ColorSpace.Gray => ImageMagick.ColorSpace.Gray,
                    ColorSpace.HSI => ImageMagick.ColorSpace.HSI,
                    ColorSpace.HSL => ImageMagick.ColorSpace.HSL,
                    ColorSpace.HSV => ImageMagick.ColorSpace.HSV,
                    ColorSpace.RGB => ImageMagick.ColorSpace.RGB,
                    ColorSpace.scRGB => ImageMagick.ColorSpace.scRGB,
                    ColorSpace.sRGB => ImageMagick.ColorSpace.sRGB,
            };
        }


        public ColorSpace ToApplicationColorSpace(ImageMagick.ColorSpace magicColorSpace)
        {
            return magicColorSpace switch
            {
                ImageMagick.ColorSpace.Undefined => ColorSpace.Undefined,
                ImageMagick.ColorSpace.Gray => ColorSpace.Gray,
                ImageMagick.ColorSpace.HSI => ColorSpace.HSI,
                ImageMagick.ColorSpace.HSL => ColorSpace.HSL,
                ImageMagick.ColorSpace.HSV => ColorSpace.HSV,
                ImageMagick.ColorSpace.RGB => ColorSpace.RGB,
                ImageMagick.ColorSpace.scRGB => ColorSpace.scRGB,
                ImageMagick.ColorSpace.sRGB => ColorSpace.sRGB,
                _ => throw new InvalidEnumArgumentException($"مقدار جاری ، در نوع {typeof(ColorSpace)} پشتیبانی نمیشود .")
            };
        }


        ///////////////////////////////////////////////////////////////////////////////////
        

        public IReadOnlyList<ImageMagick.PixelChannel> ToMagicPixelChannels(IEnumerable<PixelChannel> applicationPixelChannels)
        {
            List<ImageMagick.PixelChannel> magickPixelChannels = new List<ImageMagick.PixelChannel>();

            foreach (PixelChannel applicationPixelChannel in applicationPixelChannels)
            {
                magickPixelChannels.Add(this.ToMagicPixelChannel(applicationPixelChannel));
            }
            return magickPixelChannels.AsReadOnly();
        }


        public IReadOnlyList<PixelChannel> ToApplicationPixelChannels(IEnumerable<ImageMagick.PixelChannel> magickPixelChannels)
        {
            List<PixelChannel> applicationPixelChannels = new List<PixelChannel>();

            foreach (ImageMagick.PixelChannel magickPixelChannel in magickPixelChannels)
            {
                applicationPixelChannels.Add(this.ToApplicationPixelChannel(magickPixelChannel));
            }
            return applicationPixelChannels.AsReadOnly();
        }


        public ImageMagick.PixelChannel ToMagicPixelChannel(PixelChannel applicationPixelChannel)
        {
            return applicationPixelChannel switch
            {
                PixelChannel.Red => ImageMagick.PixelChannel.Red,
                PixelChannel.Green => ImageMagick.PixelChannel.Green,
                PixelChannel.Blue => ImageMagick.PixelChannel.Blue,
                PixelChannel.Alpha => ImageMagick.PixelChannel.Alpha,
                PixelChannel.Index => ImageMagick.PixelChannel.Index,
            };
        }


        public PixelChannel ToApplicationPixelChannel(ImageMagick.PixelChannel magicPixelChannel)
        {
            return magicPixelChannel switch
            {
                ImageMagick.PixelChannel.Red => PixelChannel.Red,
                ImageMagick.PixelChannel.Green => PixelChannel.Green,
                ImageMagick.PixelChannel.Blue => PixelChannel.Blue,
                ImageMagick.PixelChannel.Alpha => PixelChannel.Alpha,
                ImageMagick.PixelChannel.Index => PixelChannel.Index,
                _ => throw new InvalidEnumArgumentException($"مقدار جاری ، در نوع {typeof(PixelChannel)} پشتیبانی نمیشود .")
            };
        }


    }
}
