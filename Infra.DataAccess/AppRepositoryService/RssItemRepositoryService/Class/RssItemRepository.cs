using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.SharedProject.Disposed.Interface;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.AppCacheManagerService.RssItemRepositoryCacheCoordinatorService.InternalClass;
using Infra.DataAccess.AppCacheManagerService.RssItemRepositoryCacheCoordinatorService.InternalInterface;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.ExpressionFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface;
using Infra.DataAccess.RssItemReconstructorService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.AppRepositoryService.RssItemRepositoryService.Class
{
    public class RssItemRepository : RepositoryBase, IRssItemRepository
    {

        private IRssItemRepositoryCacheCoordinator rssItemCacheCoordinator;

        /// <summary>
        /// قفل مشترک برای همگام‌سازیِ کش دائم (persistenceCache) با دیتابیس.
        /// 
        /// <para>
        /// <b>جهت همگام‌سازی:</b>
        /// این قفل طراحی شده تا کش دائم را با دیتابیس هماهنگ کند — <b>نه</b> دیتابیس را 
        /// با کش. یعنی: «هر تغییری که در دیتابیس رخ می‌دهد، کش دائم هم همان لحظه 
        /// به‌روز شود.» (write-through cache)
        /// </para>
        /// 
        /// <para>
        /// <b>الگوی استفاده (سه فاز):</b>
        /// ۱. <c>DbContext_SavingChanges</c>: قبل از SaveChanges، write lock گرفته می‌شود
        ///    → همه‌ی خواننده‌های persistenceCache (کلاس‌های Query) بلاک می‌شوند.
        /// ۲. SaveChanges: دیتابیس commit می‌شود.
        /// ۳. <c>DbContext_SavedChanges</c>: کش دائم از کش موقت به‌روز می‌شود، سپس 
        ///    write lock آزاد می‌شود.
        /// در صورت شکست (<c>DbContext_SaveChangesFailed</c>)، قفل آزاد می‌شود بدون 
        /// دست‌زدن به کش — تا دیتابیس و کش هر دو روی نسخه‌ی قبلی بمانند.
        /// </para>
        /// 
        /// <para>
        /// <b>چه چیزی محافظت می‌شود؟</b>
        /// در طول SaveChanges (که کند است — I/O)، هیچ Queryی نمی‌تواند نسخه‌ی قدیمی 
        /// کش را بخواند. بدون این قفل، بین commit شدن دیتابیس و به‌روز شدن کش، یک 
        /// پنجره‌ی چند میلی‌ثانیه‌ای وجود داشت که در آن، Queryها داده‌ی قدیمی می‌دیدند.
        /// </para>
        /// 
        /// <para>
        /// <b>⚠️ هزینه:</b>
        /// در طول SaveChanges، همه‌ی Queryها بلاک می‌شوند. این trade-off آگاهانه است — 
        /// برای اطمینان از سازگاری کامل بین دیتابیس و کش، به قیمت کارایی خواندن در 
        /// لحظه‌ی ذخیره‌سازی.
        /// </para>
        /// 
        /// در متدهای Add و Update و Delete ، قفل کردن نیازی نیست چون اصلا این متدها در کش دائم نمینویسند .
        /// کش دائم ، فقط در موقع SaveChanges نوشته میشه که قفل داره .
        /// متدهای Find و کلا متدهای خوندن در این کلاس ، چون اطلاعات را از کش دائم نمیخونند ، نیاز به قفل ندارند .
        /// خوندن از کش دائم ، در کلاس های Query انجام میشه که اونجا هم از همین شی قفل ، استفاده میشه .
        /// </summary>
        private ReaderWriterLockSlim sharedPersistenceCacheLock;

        private RssItemEfEntityMapper efEntityMapper;

        private IRssItemReconstructor rssItemReconstructor;

        private IRssItemEfEntityFactory rssItemEfEntityFactory;

        private RssItemEfEntityExpressionFactory expressionFactory;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemRepository(RssDbContext rssDbContext, ICache<Uri, RssItemEfEntity> persistenceCache, 
            ReaderWriterLockSlim sharedPersistenceCacheLock, IDraftCacheWriter draftCacheWriter, 
            RssItemEfEntityMapper efEntityMapper, IRssItemReconstructor rssItemReconstructor, 
            IRssItemEfEntityFactory rssItemEfEntityFactory, RssItemEfEntityExpressionFactory expressionFactory) : base(rssDbContext)
        {
            /// شی RssItemRepositoryCacheCoordinator ، نیاز به تزریق نداره . 
            /// چون برای مدیریت طول عمر draftCache ، که به ازای هر شی RssItemRepository ، باید یک شی draftCache داشته باشه ، نباید تزریق بشه .
            this.rssItemCacheCoordinator = new RssItemRepositoryCacheCoordinator(persistenceCache, draftCacheWriter);
            this.sharedPersistenceCacheLock = sharedPersistenceCacheLock;
            this.efEntityMapper = efEntityMapper;
            this.rssItemReconstructor = rssItemReconstructor;
            this.rssItemEfEntityFactory = rssItemEfEntityFactory;
            this.expressionFactory = expressionFactory;

            rssDbContext.SavingChanges += this.DbContext_SavingChanges;
            rssDbContext.SavedChanges += this.DbContext_SavedChanges;
            rssDbContext.SaveChangesFailed += this.DbContext_SaveChangesFailed;
            rssDbContext.Disposed += this.DbContext_Disposed;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public void Add(RssItem rssItem, ImageSaveOptions imageSaveOptions)
        {
            RssItemEfEntity rssItemEfEntity = this.rssItemEfEntityFactory.CreateRssItemEfEntity(rssItem, imageSaveOptions);
            this.AddEntity(rssItemEfEntity);
            // اضافه یا به روزرسانی در کش موقت
            this.rssItemCacheCoordinator.MarkAsAddedInDraftCache(rssItem.Id, rssItemEfEntity);
        }


        public void Update(RssItem rssItem, ImageSaveOptions imageSaveOptions)
        {
            RssItemEfEntity updatedRssItemEfEntity = this.rssItemEfEntityFactory.CreateRssItemEfEntity(rssItem, imageSaveOptions);
            RssItemEfEntity oldRssItemEfEntity = this.FindRssItemEfEntityByKey(rssItem.Id.AbsoluteUri);
            this.AttachEntityToDbContext(oldRssItemEfEntity);
            /// به روزرسانی
            this.efEntityMapper.UpdateRssItemEfEntity(updatedRssItemEfEntity, oldRssItemEfEntity);
            // اضافه یا به روزرسانی در کش موقت
            this.rssItemCacheCoordinator.MarkAsModifiedInDraftCache(rssItem.Id, updatedRssItemEfEntity);
        }


        public void Delete(RssItem rssItem)
        {
            RssItemEfEntity rssItemEfEntity = this.FindRssItemEfEntityByKey(rssItem.Id.AbsoluteUri);
            this.DeleteEntity(rssItemEfEntity);
            // اضافه یا به روزرسانی در کش موقت
            this.rssItemCacheCoordinator.MarkAsDeletedInDraftCache(rssItem.Id, rssItemEfEntity);
        }


        public RssItem FindByKey(string rssItemKey)
        {
            RssItemEfEntity rssItemEfEntity = this.FindRssItemEfEntityByKey(rssItemKey);
            return this.rssItemReconstructor.Reconstruct(rssItemEfEntity);
        }


        public IEnumerable<RssItem> FindByFeedId(string rssFeedId)
        {
            List<RssItemEfEntity> rssItemEfEntities = this.FindByAttributeEntity<RssItemEfEntity>(
                this.expressionFactory.ExpressionByFeedId(rssFeedId))?.
                ToList();
            if (rssItemEfEntities == null || rssItemEfEntities.Count < 1)
                return null;
            return this.ReconstructionRssItems(rssItemEfEntities);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        private void DbContext_SavingChanges(object? sender, SavingChangesEventArgs e)
        {
            this.sharedPersistenceCacheLock.EnterWriteLock();
        }


        private void DbContext_SavedChanges(object sender, SavedChangesEventArgs e)
        {
            try { this.rssItemCacheCoordinator.CommitDraftCacheToPersistenceCache(); }
            finally { this.sharedPersistenceCacheLock.ExitWriteLock(); }
        }


        private void DbContext_SaveChangesFailed(object? sender, SaveChangesFailedEventArgs e)
        {
            this.sharedPersistenceCacheLock.ExitWriteLock();
        }


        private void DbContext_Disposed(object? sender, EventArgs e)
        {
            DbContext dbContext = sender as DbContext;
            if (dbContext != null)
            {
                dbContext.SavedChanges -= this.DbContext_SavedChanges;
            }

            IDisposed disposed = sender as IDisposed;
            if(disposed != null)
            {
                disposed.Disposed -= this.DbContext_Disposed;
            }
        }


        private RssItemEfEntity FindRssItemEfEntityByKey(string rssItemKey)
        {
            return this.FindByKeyEntity<string, RssItemEfEntity>(rssItemKey);
        }


        private IEnumerable<RssItem> ReconstructionRssItems(IEnumerable<RssItemEfEntity> rssItemEfEntities)
        {
            List<RssItem> rssItems = new List<RssItem>();
            foreach (RssItemEfEntity rssItemEfEntity in rssItemEfEntities)
            {
                RssItem rssItem = this.rssItemReconstructor.Reconstruct(rssItemEfEntity);
                if (rssItem != null)
                    rssItems.Add(rssItem);
            }
            return (rssItems.Count > 0) ? rssItems : null;
        }

    }
}
