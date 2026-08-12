using Application.InfrastructureContracts.ImageProject.SharedDataModels.Enum;
using Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Interface;
using Application.Services.ImageNormalizerServices.ImageInfoValidatorService.Class;

namespace Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Factory.Interface
{
    public interface IImageChannelSequenceNormalizerFactory<TColorDepth> where TColorDepth : struct, IConvertible
    {

        IImageChannelSequenceNormalizer<TColorDepth> Create(IEnumerable<PixelChannel> channelSequences,
            ImageInfoValidator imageInfoValidator, out bool isNeedToNormalize);

    }
}
