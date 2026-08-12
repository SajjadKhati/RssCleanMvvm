using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Interface;
using Core.ImageServices.ImageCreatorService.Class;

namespace Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Class
{
    public class RgbaToArgb8BitImageChannelSequenceNormalizer : ImageCreator<byte>, IImageChannelSequenceNormalizer<byte>
    {

        public ImageData<byte> NormalizeImageChannelSequence(ImageData<byte> imageData)
        {
            byte[,,] normalizedArgbSequenceImage = this.NormalizeImageChannelSequence(imageData.Data);
            List<PixelChannel> argbChannelsSequences = new List<PixelChannel>
            {
                PixelChannel.Alpha, PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue
            };

            ImageData<byte> normalizedArgbSequenceImageData = new ImageData<byte>
            {
                Width = imageData.Width,
                Height = imageData.Height,
                ColorDepth = imageData.ColorDepth,
                ColorSpace = imageData.ColorSpace,
                ChannelsCount = imageData.ChannelsCount,
                ChannelsSequence = argbChannelsSequences.AsReadOnly(),
                Data = normalizedArgbSequenceImage
            };
            return normalizedArgbSequenceImageData;
        }


        public byte[,,] NormalizeImageChannelSequence(byte[,,] imageData)
        {
            int imageTargetChannelCount = imageData.GetLength(2);
            byte[,,] normalizedArgbSequenceImage = this.CreateImage(imageData, imageTargetChannelCount);
            return normalizedArgbSequenceImage;
        }


        protected override byte CalculateOneChannelData(byte[,,] imageSource, int y, int x, int channel)
        {
            return (channel == 0) ? imageSource[y, x, 3] : imageSource[y, x, channel - 1];
        }

    }
}
