using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Interface;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices.ImageInfoValidatorService.Class;

namespace Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Class
{
    public class Argb8BitImageNormalizer : IImageNormalizer<byte>
    {

        private ImageInfoValidator imageInfoValidator;

        private IImageChannelCountNormalizerFactory<byte> imageChannelCountNormalizerFactory;

        private IImageChannelSequenceNormalizerFactory<byte> imageChannelSequenceNormalizerFactory;

        private readonly IReadOnlyCollection<PixelChannel> argbSequences;



        public Argb8BitImageNormalizer(ImageInfoValidator imageInfoValidator, 
            IImageChannelCountNormalizerFactory<byte> imageChannelCountNormalizerFactory,
            IImageChannelSequenceNormalizerFactory<byte> imageChannelSequenceNormalizerFactory)
        {
            this.imageInfoValidator = imageInfoValidator;
            this.imageChannelCountNormalizerFactory = imageChannelCountNormalizerFactory;
            this.imageChannelSequenceNormalizerFactory = imageChannelSequenceNormalizerFactory;

            this.argbSequences = (new List<PixelChannel>()
            {
                PixelChannel.Alpha,
                PixelChannel.Red,
                PixelChannel.Green,
                PixelChannel.Blue
            }).AsReadOnly();
        }




        public ImageData<byte> NormalizeImage(ImageData<byte> sourceImageData)
        {
            ImageData<byte> lastNormalizedImageData = sourceImageData;

            bool hasRgbColorSpace = this.imageInfoValidator.HasRgbColorSpace(lastNormalizedImageData.ColorSpace);
            if (hasRgbColorSpace == false)
                throw new NotSupportedException("در این نسخه ، فقط تصاویر با فضای رنگی RGB یا sRGB یا scRGB پشتیبانی میشود .");

            bool has8BitColorDepth = this.imageInfoValidator.Has8BitColorDepth(lastNormalizedImageData.ColorDepth);
            if (has8BitColorDepth == false)
                throw new NotSupportedException("در این نسخه ، فقط تصاویر با عمق رنگ 8 بیت پشتیبانی میشود .");

            bool has4Channels = this.imageInfoValidator.Has4Channels(lastNormalizedImageData.ChannelsCount);
            if(has4Channels == false)
            {
                var imageChannelCountNormalizer = this.imageChannelCountNormalizerFactory.Create(
                    lastNormalizedImageData.ChannelsCount, out bool isNeedToNormalize);
                if(isNeedToNormalize == true && imageChannelCountNormalizer != null)
                {
                    lastNormalizedImageData = imageChannelCountNormalizer.NormalizeImageChannelCounts(lastNormalizedImageData);
                }
            }

            bool hasArgbChannelSequence = this.imageInfoValidator.HasSpecificChannelSequence(
                lastNormalizedImageData.ChannelsSequence, this.argbSequences);
            if( hasArgbChannelSequence == false)
            {
                var imageChannelSequenceNormalizer = this.imageChannelSequenceNormalizerFactory.Create(
                    lastNormalizedImageData.ChannelsSequence, this.imageInfoValidator, out bool isNeedToNormalize);
                if (isNeedToNormalize == true && imageChannelSequenceNormalizer != null)
                {
                    lastNormalizedImageData = imageChannelSequenceNormalizer.NormalizeImageChannelSequence(lastNormalizedImageData);
                }
            }

            return lastNormalizedImageData;
        }

    }
}
