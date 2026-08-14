using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class
{
    public class ImageSettings<TImageInfo> : ImageSettingsBase
        where TImageInfo : ImageInfo
    {

        public TImageInfo ImageDetails { get; set; }


        ////////////////////////////////////////////////////////
        

        public ImageSettings()
        {

        }


        public ImageSettings(ImageSettingsBase imageSettingsBase) : base(imageSettingsBase)
        {

        }


        public ImageSettings(ImageSettingsBase imageSettingsBase, TImageInfo imageInfoType) : this(imageSettingsBase)
        {
            this.ImageDetails = imageInfoType;
        }

    }
}
