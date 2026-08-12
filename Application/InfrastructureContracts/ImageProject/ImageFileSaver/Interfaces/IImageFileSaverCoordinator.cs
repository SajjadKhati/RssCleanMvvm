using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;

namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces
{
    public interface IImageFileSaverCoordinator
    {

        DestinationPath Save8BitImageToFile(ImageFileSettings imageFileSettings, FilePath filePath);

    }
}
