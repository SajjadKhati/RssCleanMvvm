using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.SaveRssItemToDraftCache.DataModels;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemAllSources.Interface
{
    public interface IRssItemAllSourcesOrchestrator
    {

        RssItem GetRssItem(Uri rssItemId);

        RssItemDto GetRssItemDto(Uri rssItemId);

        RssItemWithDto GetRssItemWithDto(Uri rssItemId);

        bool CommitDraftCacheItemToDatabase(Uri rssItemId, ImageSaveOptions imageSaveOptions);

    }
}
