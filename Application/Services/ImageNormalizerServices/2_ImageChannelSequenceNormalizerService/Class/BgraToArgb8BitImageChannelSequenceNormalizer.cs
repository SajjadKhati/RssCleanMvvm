using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Interface;
using Core.ImageServices.ImageCreatorService.Class;

namespace Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Class
{
    public class BgraToArgb8BitImageChannelSequenceNormalizer : ImageCreator<byte>, IImageChannelSequenceNormalizer<byte>
    {

        protected int lastImageSourceIndex;

        private int _imageTargetChannelCount;


        private int ImageTargetChannelCount
        {
            get
            {
                return this._imageTargetChannelCount;
            }
            set
            {
                if(this._imageTargetChannelCount == value)
                    return;
                this._imageTargetChannelCount = value;
                this.lastImageSourceIndex = value - 1;
            }
        }



        public ImageData<byte> NormalizeImageChannelSequence(ImageData<byte> imageData)
        {
            this.ImageTargetChannelCount = imageData.Data.GetLength(2);

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
            byte[,,] normalizedArgbSequenceImage = this.CreateImage(imageData, this.ImageTargetChannelCount);
            return normalizedArgbSequenceImage;
        }


        protected override byte CalculateOneChannelData(byte[,,] imageSource, int y, int x, int channel)
        {
            return imageSource[y, x, this.lastImageSourceIndex -  channel];
        }


    }
}
