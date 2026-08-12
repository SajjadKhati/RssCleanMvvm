using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces
{
    public interface IImageDataExtractor : IImageInfoExtractor
    {

        ImageData<byte> Get8BitImageData(byte[] rawImage);

        byte[,,] Get8BitImage(byte[] rawImage);

    }
}
