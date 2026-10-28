namespace Application.UseCases.Entry.CommitAllRssItems.Interface
{
    /// <summary>
    /// commit همه‌ی آیتم‌های ذخیره‌نشده در DraftCache، بدون برگرداندن DTO.
    /// مورد استفاده‌ی App هنگام خروج از برنامه (که DTO لازم ندارد).
    /// </summary>
    public interface ICommitAllRssItemsNoResultUseCase
    {

        /// <summary>
        /// <summary>آیا آیتم ذخیره‌نشده‌ای وجود دارد؟</summary>
        /// </summary>
        /// <returns></returns>
        bool HasUnsavedItems();

        /// <summary>
        /// همه‌ی آیتم‌های ذخیره‌نشده را commit می‌کند (بدون برگرداندن نتیجه).
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();

    }
}
