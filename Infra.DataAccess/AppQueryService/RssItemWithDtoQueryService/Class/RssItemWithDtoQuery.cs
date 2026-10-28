using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemWithDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.RssItemReconstructorService.Interface;

namespace Infra.DataAccess.AppQueryService.RssItemWithDtoQueryService.Class
{
    public class RssItemWithDtoQuery : DbContextOwningQueryBase, IRssItemWithDtoQuery
    {

        private ICache<Uri, RssItemEfEntity> persistenceCache;

        /// <summary>
        /// قفل مشترک برای همگام‌سازی کش دائم (persistenceCache) با دیتابیس.
        /// 
        /// این قفل بین چند کلاس به اشتراک گذاشته می‌شود:
        /// - در RssItemRepository با write lock (برای نوشتن در کش دائم، بعد از SaveChanges).
        /// - در این کلاس و RssItemDtoQuery با read lock (برای خواندن از کش دائم).
        /// 
        /// هدف: جلوگیری از خواندن نسخه‌ی قدیمی کش، در طول پنجره‌ی بین commit دیتابیس و 
        /// به‌روزرسانی کش.
        /// </summary>
        private ReaderWriterLockSlim sharedPersistenceCacheLock;

        private IRssItemReconstructor rssItemReconstructor;

        private RssItemDtoEfEntityMapper rssItemDtoMapper;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemWithDtoQuery(RssDbContext rssDbContext, ICache<Uri, RssItemEfEntity> persistenceCache,
            ReaderWriterLockSlim sharedPersistenceCacheLock, IRssItemReconstructor rssItemReconstructor, 
            RssItemDtoEfEntityMapper rssItemDtoMapper) : base(rssDbContext)
        {
            this.persistenceCache = persistenceCache;
            this.sharedPersistenceCacheLock = sharedPersistenceCacheLock;
            this.rssItemReconstructor = rssItemReconstructor;
            this.rssItemDtoMapper = rssItemDtoMapper;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemWithDto FindByKey(string rssItemId)
        {
            /// ⚠️ قفل خواندن کل متد: چون با قفل نوشتنِ RssItemRepository مشترک است، در طول 
            /// SaveChanges بلاک می‌شود — و بعد از آن، نسخه‌ی جدید کش را می‌خواند.
            this.sharedPersistenceCacheLock.EnterReadLock();
            try
            {
                if (this.persistenceCache.TryGetValue(new Uri(rssItemId), out RssItemEfEntity rssItemEfEntity) == false || rssItemEfEntity == null)
                    rssItemEfEntity = this.FindByKeyEntity<string, RssItemEfEntity>(rssItemId);

                return (rssItemEfEntity == null) ? null : this.MapToRssItemWithDto(rssItemEfEntity);
            }
            finally { this.sharedPersistenceCacheLock.ExitReadLock(); }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        private RssItemWithDto MapToRssItemWithDto(RssItemEfEntity rssItemEfEntity)
        {
            RssItem rssItem = this.rssItemReconstructor.Reconstruct(rssItemEfEntity);
            RssItemDto rssItemDto = this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity, isUnsaved : false);
            return new RssItemWithDto(rssItem, rssItemDto);
        }

    }
}
