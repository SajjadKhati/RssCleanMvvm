using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;

namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class
{
    public class ImageSettingsBase
    {

        public StorageType StorageType { get; set; }

        public PixelMapping Mapping { get; set; }

        public AlphaOption HaveAlphaChannel { get; set; }

        public MagickFormat Format { get; set; }


        /////////////////////////////////////////////////////////
        

        public ImageSettingsBase()
        {

        }


        public ImageSettingsBase(ImageSettingsBase imageSettingsBase)
        {
            this.StorageType = imageSettingsBase.StorageType;
            this.Mapping = imageSettingsBase.Mapping;
            this.HaveAlphaChannel = imageSettingsBase.HaveAlphaChannel;
            this.Format = imageSettingsBase.Format;
        }

    }
}
