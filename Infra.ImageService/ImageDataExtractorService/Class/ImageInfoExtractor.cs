using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using ImageMagick;
using Infra.ImageService.MappingService.Class;

namespace Infra.ImageService.ImageDataExtractorService.Class
{
    public class ImageInfoExtractor : IImageInfoExtractor
    {

        private ImageInfoMemberMapper imageInfoMemberMapper;


        ///////////////////////////////////////////////////////////


        public ImageInfoExtractor(ImageInfoMemberMapper imageInfoMemberMapper)
        {
            this.imageInfoMemberMapper = imageInfoMemberMapper;
        }


        ///////////////////////////////////////////////////////////


        public ImageInfo GetImageInfo(byte[] rawImage)
        {
            using var image = new MagickImage(rawImage);
            return new ImageInfo
            {
                Width = (int)image.Width,
                Height = (int)image.Height,
                ColorDepth = (int)image.Depth,
                ColorSpace = this.imageInfoMemberMapper.ToApplicationColorSpace(image.ColorSpace),
                ChannelsCount = (int)image.ChannelCount,
                ChannelsSequence = this.imageInfoMemberMapper.ToApplicationPixelChannels(image.Channels)
            };
        }

    }
}
