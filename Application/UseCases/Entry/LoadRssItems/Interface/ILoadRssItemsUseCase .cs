using Application.UseCases.Entry.LoadRssItems.DataModels;

namespace Application.UseCases.Entry.LoadRssItems.Interface
{
    public interface ILoadRssItemsUseCase
    {
        Task<LoadRssItemsResult> ExecuteAsync(Uri rssFeedId, CancellationToken cancellationToken);
    }
}
