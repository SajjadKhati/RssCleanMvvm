using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.DataModels
{
    public class ImageSaveOptions
    {

        public ImageSettings<ImageInfo> ImageInfoSettings {  get; set; }

        public string FolderPath {  get; set; }

    }
}
