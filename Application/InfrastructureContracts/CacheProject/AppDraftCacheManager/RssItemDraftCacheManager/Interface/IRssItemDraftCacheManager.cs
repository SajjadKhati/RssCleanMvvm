using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.SharedProject.DraftCacheStateMarker.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface
{
    public interface IRssItemDraftCacheManager : IDraftCacheStateMarker<Uri, RssItem>, IDisposable
    {

        void Remove(Uri key);

        void Clear();

        bool ContainKey(Uri key);

        DraftEntry<RssItem> GetRssItemDraftEntry(Uri key);

        IReadOnlyList<DraftEntry<RssItem>> GetAllRssItemDraftEntries();

        RssItemDto GetRssItemDto(Uri key);

        IReadOnlyList<RssItemDto> GetAllRssItemDto();

    }
}
