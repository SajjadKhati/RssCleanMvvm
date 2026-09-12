using Application.UseCases.DownLoadRssItemsUseCaseModule.DataModels;

namespace Application.UseCases.DownLoadRssItemsUseCaseModule.Interface
{
    public interface IDownLoadRssItemsUseCase
    {
        Task<RssItemDtosResult> ExecuteAsync(Uri rssFeedId);
    }
}
