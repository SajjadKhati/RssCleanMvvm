using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.Entry.CommitRssItem.Interface
{
    public interface ICommitRssItemUseCase
    {
        Task<RssItemDto> ExecuteAsync(Uri rssItemId);
    }
}
