using Application.OverallDataModels.OverallDto.CompositeDto;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemWithDtoQuery.Interface
{
    /// <summary>
    /// کوئری اختصاصی برای خواندن اتمی RssItem + RssItemDto از دیتابیس.
    /// 
    /// <para>
    /// <b>چرا این سرویس به‌طور مجزا وجود دارد؟</b>
    /// اگر بخواهیم RssItem و RssItemDto را با دو کوئری جدا بخوانیم (یکی از 
    /// Repository و یکی از RssItemDtoQuery)، بین این دو کوئری یک شکاف زمانی 
    /// وجود دارد که در آن، نخ دیگری می‌تواند دیتابیس را تغییر دهد. نتیجه یک 
    /// جفت ناسازگار است: RssItem از نسخه‌ی قدیمی، RssItemDto از نسخه‌ی جدید.
    /// 
    /// این سرویس، با یک کوئری واحد (یک RssItemEfEntity) و ساخت هر دو مدل از 
    /// همان یک snapshot، سازگاری را تضمین می‌کند. قفل حافظه هم نمی‌تواند این 
    /// مسئله را حل کند — چون دیتابیس منبعی جداگانه با مدل همزمانی خودش است.
    /// </para>
    /// 
    /// <para>
    /// این سرویس، بخشی از read side (Query) است — پس خروجی آن (RssItemWithDto) 
    /// یک read model ترکیبی است و نقض DDD نیست. Repository دست‌نخورده می‌ماند 
    /// (فقط aggregate root را برمی‌گرداند).
    /// </para>
    /// </summary>
    public interface IRssItemWithDtoQuery : IDisposable
    {
        RssItemWithDto FindByKey(string rssItemId);
    }
}
