using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.DataModels;

namespace Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface
{
    public interface ISaveRssItemDataToDraftCacheUseCase
    {
        Task<RssItemDto> ExecuteAsync(RssItemDataEditableInput rssItemDataEditableInput);
    }
}
