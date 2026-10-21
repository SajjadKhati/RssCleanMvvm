using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;

namespace Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.Interface
{
    public interface IRssItemDownloaderIntoDraftCacheOrchestrator
    {
        public Task<RssItemWithDtosResult> DownloadAndStoreInDraftCacheAsync(
            RssFeedConfig rssFeedConfig, CancellationToken cancellationToken = default);
    }
}
