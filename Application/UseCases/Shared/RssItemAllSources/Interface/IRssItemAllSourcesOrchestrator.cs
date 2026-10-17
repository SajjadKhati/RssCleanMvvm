using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemAllSources.Interface
{
    public interface IRssItemAllSourcesOrchestrator
    {

        RssItem GetRssItem(Uri rssItemId);

        RssItemDto GetRssItemDto(Uri rssItemId);

        RssItemWithDto GetRssItemWithDto(Uri rssItemId);

        bool TryGetRssItemWithDtoAndDraftEntry(Uri rssItemId, out RssItemWithDto rssItemWithDto, out DraftEntry<RssItem> originalDraftEntry);

        IList<Uri> CommitAllDraftCacheItemsToDatabase();

        bool CommitDraftCacheItemToDatabase(Uri rssItemId);

        IList<Uri> CommitDraftCacheItemBatchToDatabase(IEnumerable<Uri> rssItemIds);

    }
}
