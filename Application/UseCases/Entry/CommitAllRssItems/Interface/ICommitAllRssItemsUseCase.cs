using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.Entry.CommitAllRssItems.Interface
{
    /// <summary>
    /// commit همه‌ی آیتم‌های ذخیره‌نشده در DraftCache، با برگرداندن DTOهای به‌روزشده.
    /// مورد استفاده‌ی RssUserControlViewModel برای SaveAll (با CanExecute و رویداد).
    /// </summary>
    public interface ICommitAllRssItemsUseCase
    {

        /// <summary>
        /// هر بار حالت «وجود آیتم ذخیره‌نشده» تغییر کند (true ↔ false).
        /// فقط در نقطه‌ی گذار fire می‌شود، نه روی هر تغییر در cache.
        /// مقدار bool: true = آیتم ذخیره‌نشده دارد، false = ندارد.
        /// </summary>
        event EventHandler<bool> HasUnsavedItemsChanged;

        /// <summary>
        /// <summary>آیا آیتم ذخیره‌نشده‌ای وجود دارد؟</summary>
        /// </summary>
        /// <returns></returns>
        bool HasUnsavedItems();

        /// <summary>
        /// همه‌ی آیتم‌های ذخیره‌نشده را commit می‌کند و DTOهای به‌روزشده را برمی‌گرداند.
        /// </summary>
        /// <returns></returns>
        Task<IList<RssItemDto>> ExecuteAsync();

    }
}
