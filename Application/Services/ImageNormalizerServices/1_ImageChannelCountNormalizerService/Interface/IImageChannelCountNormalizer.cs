using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.Services.ImageNormalizerServices._1_ImageChannelCountNormalizerService.Interface
{
    public interface IImageChannelCountNormalizer<TColorDepth> where TColorDepth : struct, IConvertible
    {

        ImageData<TColorDepth> NormalizeImageChannelCounts(ImageData<TColorDepth> imageData);

    }
}
