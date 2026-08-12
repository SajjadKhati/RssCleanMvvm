using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces
{
    public interface IImageInfoExtractor
    {

        ImageInfo GetImageInfo(byte[] rawImage);

    }
}
