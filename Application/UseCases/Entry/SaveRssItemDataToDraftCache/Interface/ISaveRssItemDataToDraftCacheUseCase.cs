using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.SaveRssItemDataToDraftCache.DataModels;

namespace Application.UseCases.Entry.SaveRssItemDataToDraftCache.Interface
{
    public interface ISaveRssItemDataToDraftCacheUseCase
    {
        /// <summary>
        /// داده‌های ویرایش‌شده (متن) را در DraftCache ذخیره می‌کند.
        ///
        /// ⚠️ چرا RssItemDto برگردانده می‌شود و نه bool؟
        ///
        /// ذخیره در DraftCache به دلایل مختلف ممکن است شکست بخورد (به‌ویژه
        /// optimistic concurrency check: اگر نخ دیگری زودتر Description را تغییر
        /// داده باشد).
        /// در این حالت bool = false هیچ اطلاعاتی درباره‌ی «وضعیت واقعی cache»
        /// به VM نمی‌دهد.
        ///
        /// RssItemDto (یا null) این اطلاعات را می‌دهد:
        ///   - غیر null → VM با UpdateDataFromDto، Description و IsUnsaved را با
        ///     نسخه‌ی واقعی cache هم‌گام می‌کند. اگر نخ دیگری متنی متفاوت نوشته
        ///     باشد، VM همان متن را نشان می‌دهد (نه متن ذخیره‌نشده‌ی خودش).
        ///   - null → عملیات انجام نشد (VM تصمیم می‌گیرد).
        /// </summary>
        /// <param name="rssItemDataEditableInput"></param>
        /// <returns></returns>
        Task<RssItemDto> ExecuteAsync(RssItemDataEditableInput rssItemDataEditableInput);
    }
}
