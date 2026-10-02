using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.UseCases.Shared.SharedDataModels.Class;

namespace Application.UseCases.Shared.RssItemsDownLoader.Interface
{
    public interface IRssItemsDownLoaderOrchestrator
    {
        Task<RssItemsReport> DownLoadRssItemsAsync(RssFeedConfig rssFeedConfig);
    }
}
