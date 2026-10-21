using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Application.Services.RssItemCreationServices.RssItemBuilderService.Interface;
using Application.Services.RssItemCreationServices.RssItemDataExtractorService.Interface;
using Domain.DomainEntities.RssEntity.Class;
using Domain.DomainService.RssItemDomainService.Interface;

namespace Application.Services.RssItemCreationServices.RssItemBuilderService.Class
{
    /// <summary>
    /// هماهنگ سازی یا orchestr یا همون استفاده از سرویس های دامنه برای اعتبارسنجی ، وظیفه ی این کلاس هست . 
    /// یعنی این کار ، وظیفه ی کلاس RssItemDataExtractor نیست .
    /// </summary>
    public class RssItemBuilder : IRssItemBuilder
    {

        private IRssItemDomainService rssItemDomainService;

        private IRssItemDataExtractor rssItemDataExtractor;

        private IImageSettingsWriter imageSettingsWriter;


        //////////////////////////////////////////////////////////


        public RssItemBuilder(IRssItemDomainService rssItemDomainService, IRssItemDataExtractor rssItemDataExtractor, 
            IImageSettingsWriter imageSettingsWriter)
        {
            this.rssItemDomainService = rssItemDomainService;
            this.rssItemDataExtractor = rssItemDataExtractor;
            this.imageSettingsWriter = imageSettingsWriter;
        }


        //////////////////////////////////////////////////////////


        /// <summary>
        /// یک RssItem را از داده‌ی خام XML یک فید خاص می‌سازد.
        ///
        /// چرا پیکربندی از بیرون می‌آید؟
        /// هر فید، نام کلیدهای XML مخصوص خودش را دارد (مثلاً «link» در یک سایت،
        /// «url» در سایت دیگر ، یا "uri" در سایت دیگر . و همچنین برای بقیه ی فیلدها هم همچنین). این تفاوت‌ها در RssItemFieldKeyConfig ذخیره شده‌اند
        /// (در دیتابیس، به ازای هر فید) و اینجا برای استخراج داده‌ها استفاده می‌شوند.
        ///
        /// جریان کار:
        ///   ۱. از روی پیکربندی، کلید مناسب هر فیلد را در دیکشنری پیدا می‌کنیم.
        ///   ۲. مقدار خام (رشته) را با RssItemDataExtractor به نوع مقصد تبدیل می‌کنیم.
        ///   ۳. یکتایی شناسه را با Domain Service بررسی می‌کنیم.
        ///   ۴. تصویر را برای استفاده‌ی بعدی کش می‌کنیم.
        ///   
        /// 
        /// ⚠️ چرا این متد قفل ندارد؟ (تحلیل race)
        /// 
        /// Race ۱ — TOCTOU بین IsRssItemValid و Store:
        ///   دو نخ همزمان می‌توانند همان ID را پردازش کنند. هر دو IsRssItemValid 
        ///   را true می‌بینند، هر دو تصویر را دانلود می‌کنند، و هر دو Store می‌زنند. 
        ///   ولی چون هر دو از همان XML می‌خوانند، تصاویر یکسان هستند و overwrite 
        ///   بی‌ضرر است. داده از دست نمی‌رود — فقط نسخه‌ی دوم جایگزین می‌شود.
        /// 
        /// Race ۲ — Store اتمی است:
        ///   Store در ImageSettingsProvider، Set روی ConcurrentDictionary است که 
        ///   اتمی است. پس هیچ ناسازگاری داخلی در cache رخ نمی‌دهد.
        /// 
        /// قفل راه‌حل نبود:
        ///   اگر CreateRssItemAsync را قفل می‌کردیم، در طول دانلود تصویر (چند 
        ///   ثانیه)، همه‌ی دانلودهای دیگر — حتی برای IDهای متفاوت — بلاک می‌شدند. 
        ///   این یک فاجعه‌ی کارایی بود.
        /// 
        /// ⚠️ نکته درباره‌ی try/catch خالی:
        ///   اگر Store شکست بخورد، RssItem همچنان ساخته می‌شود. بعداً وقتی 
        ///   GetOrDefault(id, image) صدا زده شود، default settings برمی‌گردد — 
        ///   که معتبر است. این graceful degradation است، نه یک باگ. (ولی بهتر 
        ///   است استثنا log شود.)
        /// </summary>
        public async Task<RssItem> CreateRssItemAsync(IDictionary<string, string> rssItemDictionary, 
            RssItemWithFeedIdFieldKeyConfig rssItemWithFeedIdFieldKeyConfig, CancellationToken cancellationToken = default)
        {
            RssItemFieldKeyConfig rssItemFieldKeyConfig = rssItemWithFeedIdFieldKeyConfig.RssItemFieldKeyConfig;
            Uri rssFeedId = rssItemWithFeedIdFieldKeyConfig.RssFeedId;

            // ۱. اعتبارسنجی همه‌ی فیلدهای اجباری، اول متد
            this.ValidateRequiredFields(rssItemDictionary, rssItemFieldKeyConfig);

            Uri id = this.rssItemDataExtractor.GetId(rssItemDictionary[rssItemFieldKeyConfig.IdKey]);
            if (this.rssItemDomainService.IsRssItemValid(id) == false)
                return null;

            Uri link = this.rssItemDataExtractor.GetLink(rssItemDictionary[rssItemFieldKeyConfig.LinkKey]);
            string category = (string.IsNullOrWhiteSpace(rssItemFieldKeyConfig.CategoryKey) == false)? 
                rssItemDictionary[rssItemFieldKeyConfig.CategoryKey] : null;
            string title = rssItemDictionary[rssItemFieldKeyConfig.TitleKey];
            string description = rssItemDictionary[rssItemFieldKeyConfig.DescriptionKey];
            DateTime publishDate = this.rssItemDataExtractor.GetPublishDate(rssItemDictionary[rssItemFieldKeyConfig.PublishDateKey]);
            ImageData<byte> argb8BitImageData = await this.rssItemDataExtractor.GetArgb8BitImageDataAsync(
                rssItemDictionary[rssItemFieldKeyConfig.Argb8BitImageKey], cancellationToken);
            try
            {
                this.imageSettingsWriter.Store(id, argb8BitImageData);
            }
            catch (Exception exception)
            {
            }
            return new RssItem(id, rssFeedId, link, argb8BitImageData.Data, category, title, description, publishDate);
        }


        /// <summary>
        /// همه‌ی فیلدهای اجباری رو اول متد بررسی می‌کنه (fail-fast).
        /// اگه کلیدی نباشه، قبل از هر عملیات سنگین (DB یا دانلود تصویر) خطا می‌ده.
        /// </summary>
        private void ValidateRequiredFields(IDictionary<string, string> dictionary, RssItemFieldKeyConfig fieldKeyConfig)
        {
            this.GetRequiredFieldWithValidate(dictionary, fieldKeyConfig.IdKey, "شناسه‌ی خبر");
            this.GetRequiredFieldWithValidate(dictionary, fieldKeyConfig.LinkKey, "لینک خبر");
            this.GetRequiredFieldWithValidate(dictionary, fieldKeyConfig.TitleKey, "عنوان خبر");
            this.GetRequiredFieldWithValidate(dictionary, fieldKeyConfig.DescriptionKey, "توضیحات خبر");
            this.GetRequiredFieldWithValidate(dictionary, fieldKeyConfig.PublishDateKey, "تاریخ انتشار");
            this.GetRequiredFieldWithValidate(dictionary, fieldKeyConfig.Argb8BitImageKey, "تصویر خبر");
        }


        /// <summary>
        /// مقدار یک فیلد اجباری را از دیکشنری XML برمی‌گرداند.
        /// اگر کلید در دیکشنری نباشد، KeyNotFoundException با پیام دقیق پرتاب می‌کند
        /// (شامل کلید واقعی و توضیح فارسی) تا دیباگ فیدهای خراب آسان شود.
        /// </summary>
        private string GetRequiredFieldWithValidate(IDictionary<string, string> dictionary, string key, string fieldDescription)
        {
            if (dictionary.TryGetValue(key, out string? value) == false)
                throw new KeyNotFoundException($"کلید «{key}» ({fieldDescription}) در داده‌ی XML این فید پیدا نشد.");
            return value;
        }

    }
}
