using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface
{
    /// <summary>
    /// خواننده ی کش‌های موقت.
    /// 
    /// <para>
    /// <b>سیاست Thread-Safety:</b>
    /// تمام متدهای public این رابط، thread-safe و اتمیک هستند.
    /// متدهایی که چند کش را با هم تغییر می‌دهند یا می‌خوانند، با
    /// ReaderWriterLockSlim داخلی محافظت می‌شوند — پس مصرف‌کننده نیازی
    /// به قفل بیرونی ندارد.
    /// </para>
    /// </summary>
    public interface IRssItemDraftCacheReader
    {

        bool ContainKey(Uri key);

        IReadOnlyCollection<Uri> GetAllDraftCacheKeys();

        bool TryGetRssItemDraftEntry(Uri key, out DraftEntry<RssItem> rssItemDraftEntry);

        IReadOnlyList<DraftEntry<RssItem>> GetAllRssItemDraftEntriesCopy();

        bool TryGetRssItemDto(Uri key, out RssItemDto rssItemDto);

        IReadOnlyList<RssItemDto> GetRssItemDtosByFeedIdCopy(Uri rssFeedId);

        bool TryGetRssItemWithDto(Uri key, out RssItemWithDto rssItemWithDto);

        bool TryGetRssItemWithDtoAndDraftEntry(Uri key, out RssItemWithDto rssItemWithDto, out DraftEntry<RssItem> rssItemDraftEntry);

    }
}
