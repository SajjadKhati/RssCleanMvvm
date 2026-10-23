using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.Entry.ConvertRssItemImageToGrayscale.Interface
{
    public interface IConvertRssItemImageToGrayscaleUseCase
    {
        /// <summary>
        /// تصویر خبر را به سیاه و سفید تبدیل می‌کند و نتیجه را در DraftCache ذخیره می‌کند.
        ///
        /// ⚠️ چرا RssItemDto برگردانده می‌شود و نه bool؟
        ///
        /// ذخیره در DraftCache ممکن است به دو دلیل شکست بخورد:
        ///   ۱. Optimistic concurrency check شکست بخورد (نخ دیگری زودتر نوشته باشد).
        ///   ۲. آیتم به هر دلیلی در دسترس نباشد.
        /// در این حالت‌ها، bool = false به VM نمی‌گوید «چه چیزی الان در cache هست».
        ///
        /// RssItemDto (یا null) این اطلاعات را به VM می‌دهد:
        ///   - غیر null → VM با UpdateImageFromDto، ImageFullFilePath و IsUnsaved را
        ///     با واقعیت cache هم‌گام می‌کند. set کردن ImageFullFilePath (حتی با مقدار
        ///     یکسان) باعث می‌شود setter مخصوص VM، OnPropertyChanged را fire کند و
        ///     UI فایل تصویر را دوباره بخواند — چون محتوای فایل تغییر کرده، حتی اگر
        ///     مسیر یکی باشد.
        ///   - null → عملیات انجام نشد (VM تصمیم می‌گیرد).
        /// </summary>
        /// <param name="rssItemId"></param>
        /// <returns></returns>
        Task<RssItemDto> ExecuteAsync(Uri rssItemId);
    }
}
