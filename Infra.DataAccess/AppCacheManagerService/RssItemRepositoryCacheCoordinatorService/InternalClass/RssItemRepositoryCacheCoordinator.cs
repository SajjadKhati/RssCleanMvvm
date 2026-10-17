using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels;
using Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.Interfaces;
using Infra.CacheService.CacheBaseServices.DraftCacheServices.Class;
using Infra.DataAccess.AppCacheManagerService.RssItemRepositoryCacheCoordinatorService.InternalInterface;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.AppCacheManagerService.RssItemRepositoryCacheCoordinatorService.InternalClass
{
    /// <summary>
    /// هماهنگ‌کننده‌ی نوشتن در کش دائم (persistenceCache) و کش موقت (draftCache).
    /// 
    /// جای اصلی این کلاس ، در پروژه ی Infra.CacheService هست .
    /// منتها چون این کلاس ، مستقیما توسط کلاس RssItemRepository شی اش ساخته میشه (و رابط IRssItemRepositoryCacheCoordinator ، در سازنده ی RssItemRepository تزریق نمیشه) ، 
    /// بنابراین اگر این سرویس را در پروژه ی Infra.CacheService تعریف کنیم ، چرخه ی وابستگی به پروژه ها بوجود میاد و نمیشه .
    /// 
    /// ═══════════════════════════════════════════════════════════════════════
    /// ⚠️ قاعده: این کلاس هیچ قفلی لازم ندارد. دلایل:
    /// 
    /// ۱. کش موقت (draftCache):
    ///    - per-instance است (با هر شی RssItemRepository، یک شی جدید ساخته می‌شود).
    ///    - RssItemRepository به ازای هر متد/نخ ساخته می‌شود.
    ///    - پس draftCache بین نخ‌ها به اشتراک گذاشته نمی‌شود → قفل لازم ندارد.
    /// 
    /// ۲. کش دائم (persistenceCache):
    ///    - یک شیء ConcurrentDictionary است (از DI تزریق می‌شود).
    ///    - تمام عملیات روی آن (AddOrUpdate, TryRemove) تک‌عملیات اتمی هستند.
    ///    - اتمی بودن برای خواندن/نوشتن همزمان کافی است → قفل لازم ندارد.
    ///    - مصرف‌کننده‌ها (RssItemDtoQuery) هم فقط تک‌عملیات انجام می‌دهند → نیازی به قفل مشترک نیست.
    /// 
    /// ۳. متدهای این کلاس:
    ///    - MarkAsAdded/Modified/Deleted: تک‌نوشتن در draftCache.
    ///    - CommitDraftCacheToPersistenceCache: حلقه روی draftCache per-instance + 
    ///      عملیات‌های اتمی روی persistenceCache.
    ///    - هیچ invariant چندعملیاتیِ سطح‌بالاتری وجود ندارد که به قفل نیاز داشته باشد.
    /// ═══════════════════════════════════════════════════════════════════════
    /// </summary>
    internal class RssItemRepositoryCacheCoordinator : IRssItemRepositoryCacheCoordinator
    {

        /// <summary>
        /// کش دائم هست که فقط بعد از عملیات Save ، نوشته میشه . و فقط یک شی واحد از این ، به کلاس های مورد نظر ، از جمله این کلاس و همچنین کلاس RssItemDtoQuery ، تزریق میشه
        /// 
        ///  این کش ، موقع فراخوانیِ متدهای خواندن در این کلاس ، استفاده نمیشه . یعنی موقع فراخوانی متد Find در این کلاس ، از این کش استفاده نمیکنیم .
        ///  بلکه این کش ، فقط در این کلاس نوشته میشه و فقط در کلاس RssItemDtoQuery ، خونده میشه .
        ///  
        /// بنابراین نیازی به Attach کردن شی های کش به DbContext در این سناریو نیست .
        /// اما اگه سناریویی بود که موقع خوندن و تغییر دادن (هر دو با هم) ، نیاز به کش بود ، قبل از انجام کارهای دیتابیس ، باید شیِ اون کش را اول به DbContext مون ، Attach کنیم تا تغییرات را Track کنه .
        /// </summary>
        private ICache<Uri, RssItemEfEntity> persistenceCache;

        /// <summary>
        /// این کش ، کش موقت هست . بنابراین با هر شیِ جدید از این کلاس ، شی جدید از این کش لازم هست . بنابراین در سازنده ، تزریق نمیشه و در سازنده ، هر بار شی جدید ازش ساخته میشه .
        /// این کش موقت ، در متدهای Add و Delete و Update ، نوشتته میشه و در متد Save خونده و به persistenceCache منتقل میشه .
        /// 
        /// کش موقت — per-instance (با هر شیء از این کلاس، یک شیء جدید).
        /// 
        /// ⚠️ قاعده: این فیلد نیازی به قفل ندارد.
        /// دلیل: چون per-instance است و RssItemRepository (که این کلاس را می‌سازد)
        ///       به ازای هر متد/نخ ساخته می‌شود، پس هیچ نخ دیگری به این draftCache دسترسی ندارد.
        /// </summary>
        private IDraftCache<Uri, RssItemEfEntity> draftCache;

        private IDraftCacheWriter draftCacheWriter;


        ////////////////////////////////////////////////////////////////////////////////////////////


        internal RssItemRepositoryCacheCoordinator(ICache<Uri, RssItemEfEntity> persistenceCache, IDraftCacheWriter draftCacheWriter)
        {
            this.persistenceCache = persistenceCache;
            this.draftCache = new DraftCache<Uri, RssItemEfEntity>();
            this.draftCacheWriter = draftCacheWriter;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public void MarkAsAddedInDraftCache(Uri key, RssItemEfEntity rssItemEfEntity)
        {
            this.draftCacheWriter.AddOrUpdateToDraftCache(this.draftCache, key, rssItemEfEntity, DraftState.Added);
        }


        public void MarkAsModifiedInDraftCache(Uri key, RssItemEfEntity rssItemEfEntity)
        {
            this.draftCacheWriter.AddOrUpdateToDraftCache(this.draftCache, key, rssItemEfEntity, DraftState.Modified);
        }


        public void MarkAsDeletedInDraftCache(Uri key, RssItemEfEntity rssItemEfEntity)
        {
            this.draftCacheWriter.AddOrUpdateToDraftCache(this.draftCache, key, rssItemEfEntity, DraftState.Deleted);
        }


        /// <summary>
        /// انتقال تمام آیتم‌های کش موقت به کش دائم.
        /// 
        /// ⚠️ این متد نیازی به قفل ندارد:
        /// - حلقه روی GetAllValuesCopy() (که خودش snapshot می‌گیرد) → امن.
        /// - MoveDraftCacheToPersistenceCache: تک‌عملیات اتمی روی persistenceCache.
        /// - TryRemove روی draftCache per-instance: فقط همان نخ دسترسی دارد.
        /// 
        /// کل عملیات، با نخ‌های دیگر روی persistenceCache موازی است — 
        /// ولی چون هر عملیات اتمی است، هیچ داده‌ی ناهمگونی رخ نمی‌دهد.
        /// </summary>
        public void CommitDraftCacheToPersistenceCache()
        {
            foreach (DraftEntry<RssItemEfEntity> draftEntry in this.draftCache.GetAllValuesCopy())
            {
                Uri key = (Uri)draftEntry.Tag;
                this.MoveDraftCacheToPersistenceCache(key, draftEntry);
                this.draftCache.TryRemove(key);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// انتقال یک draftEntry به کش دائم، بر اساس وضعیت آن (Added/Modified/Deleted).
        /// 
        /// ⚠️ این متد نیازی به قفل ندارد:
        /// - AddOrUpdate و TryRemove روی ConcurrentDictionary، تک ‌عملیات اتمی هستند.
        /// - اتمی بودن برای هماهنگی با نخ‌های خواننده (RssItemDtoQuery) کافی است.
        /// - هیچ قانون یا invariant چندعملیاتی در این متد وجود ندارد.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="draftEntry"></param>
        private void MoveDraftCacheToPersistenceCache(Uri key, DraftEntry<RssItemEfEntity> draftEntry)
        {
            switch (draftEntry.DraftState)
            {
                case DraftState.Added:
                case DraftState.Modified:
                {
                    this.persistenceCache.AddOrUpdate(key, draftEntry.Item);
                    break;
                }
                case DraftState.Deleted:
                {
                    this.persistenceCache.TryRemove(key);
                    break;
                }
            }
        }

    }
}
