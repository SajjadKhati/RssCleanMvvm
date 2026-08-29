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
    /// جای اصلی این کلاس ، در پروژه ی Infra.CacheService هست .
    /// منتها چون این کلاس ، مستقیما توسط کلاس RssItemRepository شی اش ساخته میشه (و رابط IRssItemRepositoryCacheCoordinator ، در سازنده ی RssItemRepository تزریق نمیشه) ، 
    /// بنابراین اگر این سرویس را در پروژه ی Infra.CacheService تعریف کنیم ، چرخه ی وابستگی به پروژه ها بوجود میاد و نمیشه .
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


        public void CommitDraftCacheToPersistenceCache()
        {
            foreach (DraftEntry<RssItemEfEntity> draftEntry in this.draftCache.GetAll())
            {
                Uri key = (Uri)draftEntry.Tag;
                this.MoveDraftCacheToPersistenceCache(key, draftEntry);
                this.draftCache.Remove(key);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


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
                    this.persistenceCache.Remove(key);
                    break;
                }
            }
        }

    }
}
