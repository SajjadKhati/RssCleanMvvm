using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Infra.SharedService.ImageSettingsMapperService.Class
{
    public class ImageSettingsMapper
    {

        public ImageSettings<ImageData<TColorDepth>> MappToImageDataSettings<TColorDepth>(ImageSettings<ImageInfo> imageInfoSettings, TColorDepth[,,] image)
            where TColorDepth : struct, IConvertible
        {
            ImageData<TColorDepth>  imageData = new ImageData<TColorDepth>(imageInfoSettings.ImageDetails, image);
            ImageSettings<ImageData<TColorDepth>> imageDataSettings = new ImageSettings<ImageData<TColorDepth>>(imageInfoSettings, imageData);
            return imageDataSettings;
        }

    }
}
