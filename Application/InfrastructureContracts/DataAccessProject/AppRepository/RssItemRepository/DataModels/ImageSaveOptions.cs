using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels
{
    public class ImageSaveOptions
    {

        public ImageSettings<ImageInfo> ImageInfoSettings {  get; set; }

        public string FolderPath {  get; set; }

    }
}
