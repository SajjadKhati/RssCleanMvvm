using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces
{
    public interface IImageSettingsReader
    {

        ImageSettings<ImageInfo> Get(Uri rssItemId);

        ImageSettings<ImageInfo> GetDefault(byte[,,] image);

        ImageSettings<ImageInfo> GetOrDefault(Uri rssItemId, byte[,,] image);

    }
}
