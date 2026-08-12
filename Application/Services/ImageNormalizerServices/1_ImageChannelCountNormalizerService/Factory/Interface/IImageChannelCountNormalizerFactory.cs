using Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Interface;

namespace Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Factory.Interface
{
    public interface IImageChannelCountNormalizerFactory<TColorDepth> where TColorDepth : struct, IConvertible
    {

        IImageChannelCountNormalizer<TColorDepth> Create(int channelCounts, out bool isNeedToNormalize);

    }
}
