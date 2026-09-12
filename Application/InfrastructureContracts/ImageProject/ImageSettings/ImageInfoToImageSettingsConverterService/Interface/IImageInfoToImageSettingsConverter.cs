using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageSettings.ImageInfoToImageSettingsConverterService.Interface
{
    public interface IImageInfoToImageSettingsConverter
    {
        ImageSettingsBase Convert(ImageInfo imageInfo);
    }
}
