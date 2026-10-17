using Application.OverallDataModels.OverallDto.CompositeDto;

namespace Application.UseCases.Shared.SaveRssItemToDraftCache.Interface
{
    public interface ISaveRssItemToDraftCacheOrchestrator
    {
        bool SaveToDraftCache(Uri rssItemId, Func<RssItemWithDto, RssItemWithDto> updateFunc);
    }
}
