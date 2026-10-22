using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.Shared.SaveRssItemToDraftCache.Interface
{
    public interface ISaveRssItemToDraftCacheOrchestrator
    {
        RssItemDto SaveToDraftCache(Uri rssItemId, Func<RssItemWithDto, RssItemWithDto> updateFunc);
    }
}
