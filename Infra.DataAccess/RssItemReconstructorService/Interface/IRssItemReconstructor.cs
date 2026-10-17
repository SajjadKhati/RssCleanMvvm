using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.RssItemReconstructorService.Interface
{
    /// <summary>
    /// این رابط ، احتمالا هیچ وقت در لایه ی Application استفاده نمیشه .
    /// برای همین ، در همینجا و در لایه ی Infra و DataAccess تعریف شد .
    /// و چون نیاز به تزریقِ این رابط به سرویس های دیگه هست و بنابراین نیاز به دسترسیِ لایه ی CompositionRoot هست ، بنابراین سطح دسترسیِ internal را برایش تعریف نکردیم .
    /// </summary>
    public interface IRssItemReconstructor
    {
        RssItem Reconstruct(RssItemEfEntity rssItemEfEntity);
    }
}
