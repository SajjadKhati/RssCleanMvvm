using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces
{
    public interface IImageFileSaver
    {

        void Save8BitImageToFile(ImageFileSaveSettings imageFileSaveSettings);

    }
}
