using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;

namespace Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface
{
    public interface IRssItemDownloaderIntoDraftCacheOrchestrator
    {
        Task<RssItemsWithDtoReport> DownloadAndStoreInDraftCacheAsync(RssFeedConfig rssFeedConfig);
    }
}
