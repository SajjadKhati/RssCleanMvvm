namespace Application.InfrastructureContracts.XmlProject.SharedDataModels.Class
{
    /// <summary>
    /// نقشه‌ی نگاشت کلیدهای XML به فیلدهای RssItem، برای یک فید خاص.
    ///
    /// چرا لازم است؟
    /// هر خبرگزاری در XML خودش از نام‌های متفاوتی برای یک مفهوم استفاده می‌کند.
    /// مثلاً یکی برای لینک از lt;linkgt; استفاده می‌کند، دیگری از lt;urlgt; یا lt;Linkgt;.
    /// پس نمی‌توان نام فیلدها را در کد هاردکد کرد.
    ///
    /// این کلاس، این تفاوت‌ها را در قالب داده نگه می‌دارد.
    /// برای هر فید، یک نمونه از این کلاس در دیتابیس ذخیره می‌شود و
    /// هنگام پارس XML، از روی آن، کلیدهای مناسب همان فید استخراج می‌شود.
    /// </summary>
    public class RssItemFieldKeyConfig
    {

        public string IdKey {  get; set; }

        public string LinkKey { get; set; }

        public string CategoryKey { get; set; }

        public string TitleKey { get; set; }

        public string DescriptionKey { get; set; }

        public string PublishDateKey { get; set; }

        public string Argb8BitImageKey { get; set; }

    }
}
