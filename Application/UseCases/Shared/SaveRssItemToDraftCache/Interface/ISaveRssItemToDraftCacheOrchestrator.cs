using Application.UseCases.Shared.SaveRssItemToDraftCache.DataModels;

namespace Application.UseCases.Shared.SaveRssItemToDraftCache.Interface
{
    public interface ISaveRssItemToDraftCacheOrchestrator
    {
        void SaveToDraftCache(Uri rssItemId, Func<RssItemWithDto, RssItemWithDto> updateFunc);
    }
}
