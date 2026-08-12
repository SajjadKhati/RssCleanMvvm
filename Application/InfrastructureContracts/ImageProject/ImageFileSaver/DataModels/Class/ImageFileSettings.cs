using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class
{
    public class ImageFileSettings
    {

        public StorageType StorageType { get; set; }

        public PixelMapping Mapping { get; set; }

        public AlphaOption HaveAlphaChannel { get; set; }

        public MagickFormat Format { get; set; }

        public ImageData<byte> ImageData { get; set; }

    }
}
