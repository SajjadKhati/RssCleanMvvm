using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.Services.SharedDataModels.Class;

namespace Application.Services.RssItemsDownLoaderService.Interface
{
    public interface IRssItemsDownLoader
    {
        Task<RssItemsBuildingResult> DownLoadRssItemsAsync(RssFeedConfig rssFeedConfig);
    }
}
