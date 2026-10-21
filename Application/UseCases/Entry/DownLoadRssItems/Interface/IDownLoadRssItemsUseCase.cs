using Application.UseCases.Entry.DownLoadRssItems.DataModels;

namespace Application.UseCases.Entry.DownLoadRssItems.Interface
{
    public interface IDownLoadRssItemsUseCase
    {
        Task<RssItemDtosResult> ExecuteAsync(Uri rssFeedId, CancellationToken cancellationToken = default);
    }
}
