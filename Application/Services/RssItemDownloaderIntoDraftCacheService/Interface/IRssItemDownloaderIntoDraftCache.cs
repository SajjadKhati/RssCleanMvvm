using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.Services.RssItemDownloaderIntoDraftCacheService.DataModels;

namespace Application.Services.RssItemDownloaderIntoDraftCacheService.Interface
{
    public interface IRssItemDownloaderIntoDraftCache
    {
        Task<RssItemsWithDtoBuildingResult> DownloadAndStoreInDraftCacheAsync(RssFeedConfig rssFeedConfig);
    }
}
