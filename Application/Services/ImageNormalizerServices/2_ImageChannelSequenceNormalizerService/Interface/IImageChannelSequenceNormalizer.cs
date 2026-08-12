using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.Services.ImageNormalizerServices._2_ImageChannelSequenceNormalizerService.Interface
{
    public interface IImageChannelSequenceNormalizer<TColorDepth> where TColorDepth : struct, IConvertible
    {

        ImageData<TColorDepth> NormalizeImageChannelSequence(ImageData<TColorDepth> imageData);

    }
}
