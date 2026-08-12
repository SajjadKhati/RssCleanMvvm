using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Class;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Interface;
using Application.Services.ImageNormalizerServices.ImageInfoValidatorService.Class;

namespace Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Class
{
    public class ImageChannelSequenceNormalizerFactory : IImageChannelSequenceNormalizerFactory<byte>
    {

        private readonly IReadOnlyCollection<PixelChannel> argbSequences = new List<PixelChannel>()
        {
            PixelChannel.Alpha,
            PixelChannel.Red,
            PixelChannel.Green,
            PixelChannel.Blue
        };

        private readonly IReadOnlyCollection<PixelChannel> rgbaSequences = new List<PixelChannel>()
        {
            PixelChannel.Red,
            PixelChannel.Green,
            PixelChannel.Blue,
            PixelChannel.Alpha
        };

        private readonly IReadOnlyCollection<PixelChannel> bgraSequences = new List<PixelChannel>()
        {
            PixelChannel.Blue,
            PixelChannel.Green,
            PixelChannel.Red,
            PixelChannel.Alpha,
        };

        private readonly IReadOnlyCollection<PixelChannel> abgrSequences = new List<PixelChannel>()
        {
            PixelChannel.Alpha,
            PixelChannel.Blue,
            PixelChannel.Green,
            PixelChannel.Red
        };


        public IImageChannelSequenceNormalizer<byte> Create(IEnumerable<PixelChannel> channelSequences,
            ImageInfoValidator imageInfoValidator, out bool isNeedToNormalize)
        {
            isNeedToNormalize = true;
            bool hasArgbSequence = imageInfoValidator.HasSpecificChannelSequence(channelSequences, this.argbSequences);
            if (hasArgbSequence == true)
            {
                isNeedToNormalize = false;
                return null;
            }

            bool hasRgbASequence = imageInfoValidator.HasSpecificChannelSequence(channelSequences, this.rgbaSequences);
            if(hasRgbASequence == true)
                return new RgbaToArgb8BitImageChannelSequenceNormalizer();

            bool hasBgraASequence = imageInfoValidator.HasSpecificChannelSequence(channelSequences, this.bgraSequences);
            if (hasBgraASequence == true)
                return new BgraToArgb8BitImageChannelSequenceNormalizer();

            bool hasAbgrASequence = imageInfoValidator.HasSpecificChannelSequence(channelSequences, this.abgrSequences);
            if (hasAbgrASequence == true)
                return new AbgrToArgb8BitImageChannelSequenceNormalizer();

            throw new NotSupportedException("در این نسخه ، فقط تصاویری که ترتیب کانال های ARGB یا RGBA یا BGRA یا ABGR دارند ، پشتیبانی میشوند .");
        }

    }
}
