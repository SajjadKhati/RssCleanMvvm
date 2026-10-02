using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemDtoFactory.Interface
{
    public interface IRssItemDtoFactoryOrchestrator
    {
        RssItemDto CreateRssItemDto(RssItem rssItem);
    }
}
