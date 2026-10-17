using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Interface
{
    public interface IDraftRssItemDtoFactoryOrchestrator
    {
        RssItemDto CreateRssItemDtoWithSaveImage(RssItem rssItem);
    }
}
