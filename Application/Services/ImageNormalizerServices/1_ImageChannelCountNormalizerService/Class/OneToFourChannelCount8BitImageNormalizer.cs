using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Interface;
using Core.ImageServices.ImageCreatorService.Class;

namespace Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Class
{
    public class OneToFourChannelCount8BitImageNormalizer : ImageCreator<byte>, IImageChannelCountNormalizer<byte>
    {

        public ImageData<byte> NormalizeImageChannelCounts(ImageData<byte> imageData)
        {
            byte[,,] normalized4ChannelImage = this.NormalizeImageChannelCounts(imageData.Data);
            List<PixelChannel> argbChannelsSequences = new List<PixelChannel>
            {
                PixelChannel.Alpha, PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue 
            };

            ImageData<byte> normalized4ChannelImageData = new ImageData<byte>
            {
                Width = imageData.Width,
                Height = imageData.Height,
                ColorDepth = imageData.ColorDepth,
                ColorSpace = imageData.ColorSpace,
                ChannelsCount = 4,
                ChannelsSequence = argbChannelsSequences.AsReadOnly(),
                Data = normalized4ChannelImage
            };
            return normalized4ChannelImageData;
        }


        public byte[,,] NormalizeImageChannelCounts(byte[,,] imageData)
        {
            int normalizeImageChannelCount = 4;
            var normalized4ChannelImage = this.CreateImage(imageData, normalizeImageChannelCount);
            return normalized4ChannelImage;
        }


        protected override byte CalculateOneChannelData(byte[,,] imageSource, int y, int x, int channel)
        {
            return (channel > 0) ? imageSource[y, x, 0] : (byte)255;
        }

    }
}
