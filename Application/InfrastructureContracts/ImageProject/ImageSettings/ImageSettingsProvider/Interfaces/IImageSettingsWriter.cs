using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces
{
    public interface IImageSettingsWriter
    {
        void Store(Uri rssItemId, ImageInfo imageInfo);
    }
}
