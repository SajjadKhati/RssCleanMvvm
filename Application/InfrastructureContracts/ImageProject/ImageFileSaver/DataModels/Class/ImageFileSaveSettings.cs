using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class
{
    public class ImageFileSaveSettings
    {

        public ImageSettings<ImageData<byte>> ImageFileSettings { get; set; }

        public string FullFilePath { get; set; }

    }

}
