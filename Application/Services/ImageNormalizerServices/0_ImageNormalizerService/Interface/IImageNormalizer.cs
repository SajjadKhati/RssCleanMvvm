using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.Services.ImageNormalizerServices._0_ImageNormalizerService.Interface
{
    public interface IImageNormalizer<TColorDepth> where TColorDepth : struct, IConvertible
    {

        ImageData<TColorDepth> NormalizeImage(ImageData<TColorDepth> sourceImageData);

    }
}
