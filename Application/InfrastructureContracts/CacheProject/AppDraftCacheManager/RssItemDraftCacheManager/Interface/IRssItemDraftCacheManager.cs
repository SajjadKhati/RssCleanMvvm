using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface
{
    public interface IRssItemDraftCacheManager : IDisposable
    {

        void MarkAsAddedInDraftCache(Uri key, RssItem rssItem, RssItemDto rssItemDto);

        void MarkAsModifiedInDraftCache(Uri key, RssItem rssItem, RssItemDto rssItemDto);

        void MarkAsDeletedInDraftCache(Uri key, RssItem rssItem, RssItemDto rssItemDto);

        void Remove(Uri key);

        void Clear();

        bool ContainKey(Uri key);

        DraftEntry<RssItem> GetRssItemDraftEntry(Uri key);

        IReadOnlyList<DraftEntry<RssItem>> GetAllRssItemDraftEntries();

        RssItemDto GetRssItemDto(Uri key);

        IReadOnlyList<RssItemDto> GetAllRssItemDto();

    }
}
