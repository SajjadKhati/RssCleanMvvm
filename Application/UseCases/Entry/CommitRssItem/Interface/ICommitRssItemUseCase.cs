using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.Entry.CommitRssItem.Interface
{
    public interface ICommitRssItemUseCase
    {
        /// <summary>
        /// آیتم مشخص‌شده را از DraftCache به دیتابیس منتقل می‌کند.
        ///
        /// ⚠️ چرا RssItemDto برگردانده می‌شود و نه bool؟
        ///
        /// در فرآیند commit، تصویر آیتم از پوشه‌ی موقت (temp) به پوشه‌ی دائمی
        /// (persistent) منتقل می‌شود. در نتیجه، ImageFullFilePath آیتم تغییر
        /// می‌کند — از مسیر temp به مسیر persistent.
        ///
        /// VM به این مسیر جدید نیاز دارد؛ چون فایل temp بعد از commit دیگر
        /// وجود ندارد (منتقل شده) و اگر VM مسیر قدیمی را نگه دارد، تصویر در UI
        /// broken نمایش داده می‌شود.
        ///
        /// برگرداندن bool، این اطلاعات را به VM نمی‌رساند و VM مجبور می‌شد
        /// جداگانه از DB کوئری بگیرد — که تکرار و پیچیدگی است.
        ///
        /// RssItemDto برگشتی، نسخه‌ی ذخیره‌شده در دیتابیس را بازتاب می‌دهد
        /// (با ImageFullFilePath مسیر persistent). VM با UpdateImageFromDto،
        /// مسیر تصویر و IsUnsaved را با آن هم‌گام می‌کند. set کردن
        /// ImageFullFilePath در VM (حتی با محتوای یکسان، اما مسیر متفاوت)
        /// باعث می‌شود OnPropertyChanged صدا زده شود و UI فایل جدید را
        /// دوباره بخواند.
        ///
        /// خروجی null یعنی commit انجام نشد (آیتم در DraftCache نبود، یا
        /// optimistic check شکست خورد).
        /// </summary>
        /// <param name="rssItemId"></param>
        /// <returns></returns>
        Task<RssItemDto> ExecuteAsync(Uri rssItemId);
    }
}
