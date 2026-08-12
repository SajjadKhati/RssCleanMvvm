using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Interface;
using Core.ImageServices.ImageCreatorService.Class;
using Application.Services.CollectionService.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Class
{
    public class ThreeToFourChannelCount8BitImageNormalizer : ImageCreator<byte>, IImageChannelCountNormalizer<byte>
    {

        public ImageData<byte> NormalizeImageChannelCounts(ImageData<byte> imageData)
        {
            byte[,,] normalized4ChannelImage = this.NormalizeImageChannelCounts(imageData.Data);

            ImageData<byte> normalized4ChannelImageData = new ImageData<byte>
            {
                Width = imageData.Width,
                Height = imageData.Height,
                ColorDepth = imageData.ColorDepth,
                ColorSpace = imageData.ColorSpace,
                ChannelsCount = 4,
                ChannelsSequence = imageData.ChannelsSequence.AddToFirstItem(PixelChannel.Alpha),
                Data = normalized4ChannelImage
            };
            return normalized4ChannelImageData;
        }


        public byte[,,] NormalizeImageChannelCounts(byte[,,] imageData)
        {
            int normalizeImageChannelCount = 4;
            var normalizedArgbImage = this.CreateImage(imageData, normalizeImageChannelCount);
            return normalizedArgbImage;
        }


        protected override byte CalculateOneChannelData(byte[,,] imageSource, int y, int x, int channel)
        {
            int imageSourcePreviousChannel = channel - 1;
            return (channel > 0) ? imageSource[y, x, imageSourcePreviousChannel] : (byte)255;
        }


    }
}
