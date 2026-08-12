using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Class;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Interface;
using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Interface;

namespace Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Class
{
    public class ImageChannelCountNormalizerFactory : IImageChannelCountNormalizerFactory<byte>
    {

        public IImageChannelCountNormalizer<byte> Create(int channelCounts, out bool isNeedToNormalize)
        {
            isNeedToNormalize = true;
            if(channelCounts == 4)
            {
                isNeedToNormalize = false;
                return null;
            }
            if (channelCounts > 4 || channelCounts == 2)
                throw new NotSupportedException("در این نسخه ، تعداد بالاتر از 4 کاناله و همچنین 2 کاناله ها، پشتیبانی نمیشود .\nفقط تصاویر 1 و 3 و 4 کاناله پشتیبانی میشوند .");

            return channelCounts switch
            {
                1 => new OneToFourChannelCount8BitImageNormalizer(),
                3 => new ThreeToFourChannelCount8BitImageNormalizer(),
            };
        }

    }
}
