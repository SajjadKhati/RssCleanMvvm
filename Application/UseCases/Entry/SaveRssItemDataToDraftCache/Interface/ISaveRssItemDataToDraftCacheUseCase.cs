using Application.UseCases.Entry.SaveRssItemDataToDraftCache.DataModels;

namespace Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface
{
    public interface ISaveRssItemDataToDraftCacheUseCase
    {
        Task<bool> ExecuteAsync(RssItemDataEditableInput rssItemDataEditableInput);
    }
}
