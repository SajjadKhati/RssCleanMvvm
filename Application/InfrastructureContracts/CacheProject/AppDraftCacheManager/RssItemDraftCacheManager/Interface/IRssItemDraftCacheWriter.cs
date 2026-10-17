using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.RssItemDraftCacheManager.Interface
{
    /// <summary>
    /// نویسنده برای مدیریت‌کننده‌ی کش‌های موقت.
    /// 
    /// <para>
    /// <b>سیاست Thread-Safety:</b>
    /// تمام متدهای public این رابط، thread-safe و اتمیک هستند.
    /// متدهایی که چند کش را با هم تغییر می‌دهند یا می‌خوانند، با
    /// ReaderWriterLockSlim داخلی محافظت می‌شوند — پس مصرف‌کننده نیازی
    /// به قفل بیرونی ندارد.
    /// </para>
    /// </summary>
    public interface IRssItemDraftCacheWriter
    {

        void MarkAsAddedInDraftCache(Uri key, RssItemWithDto rssItemWithDto);

        void MarkAsModifiedInDraftCache(Uri key, RssItemWithDto rssItemWithDto);

        void MarkAsDeletedInDraftCache(Uri key, RssItemWithDto rssItemWithDto);

        bool TryMarkAsAddedIfDraftEntryUnchanged(Uri key, RssItemWithDto rssItemWithDto, DraftEntry<RssItem> originalDraftEntry);

        bool TryMarkAsModifiedIfDraftEntryUnchanged(Uri key, RssItemWithDto rssItemWithDto, DraftEntry<RssItem> originalDraftEntry);

        bool TryMarkAsDeletedIfDraftEntryUnchanged(Uri key, RssItemWithDto rssItemWithDto, DraftEntry<RssItem> originalDraftEntry);

        bool TryRemove(Uri key);

        /// <summary>
        /// حذف draft از هر دو کش — فقط اگر مقدار فعلی، همان <paramref name="originalDraftEntry"/> باشد.
        /// 
        /// ⚠️ چرا این متد لازم است؟
        /// در سناریوی Commit، بین «خواندن draft» و «حذف آن»، یک عملیات DB (کند) هست.
        /// اگر در این فاصله، نخ دیگری draft را به‌روز کند (MarkAsModified)، حذف ساده 
        /// باعث از دست رفتن نسخه‌ی جدید می‌شود. این متد فقط وقتی حذف می‌کند که نسخه 
        /// عوض نشده باشد — پس نسخه‌ی جدید برای commit بعدی باقی می‌ماند.
        /// 
        /// ⚠️ «Unchanged» یعنی «همان reference» — نه مقایسه‌ی محتوایی. چون DraftEntry 
        /// immutable است، هر تغییر = شیء جدید.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="originalDraftEntry"></param>
        /// <returns></returns>
        bool TryRemoveIfEntryUnchanged(Uri key, DraftEntry<RssItem> originalDraftEntry);

        void Clear();

    }
}
