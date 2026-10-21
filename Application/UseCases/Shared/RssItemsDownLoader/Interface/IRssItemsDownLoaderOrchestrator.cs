using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemsDownLoader.Interface
{
    public interface IRssItemsDownLoaderOrchestrator
    {
        IAsyncEnumerable<RssItem> DownLoadRssItemsAsync(RssFeedConfig rssFeedConfig, CancellationToken cancellationToken = default);
    }
}
