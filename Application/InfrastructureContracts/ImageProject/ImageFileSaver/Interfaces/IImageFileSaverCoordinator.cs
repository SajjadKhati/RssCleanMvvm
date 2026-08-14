using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;

namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces
{
    public interface IImageFileSaverCoordinator
    {

        DestinationPath Save8BitImageToFile(byte[,,] image, ImageSettings<ImageInfo> imageSettings, FilePath filePath);

    }
}
