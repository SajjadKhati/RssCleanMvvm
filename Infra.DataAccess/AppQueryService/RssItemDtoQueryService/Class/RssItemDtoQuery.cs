using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.ExpressionFactoryService.Class;

namespace Infra.DataAccess.AppQueryService.RssItemDtoQueryService.Class
{
    public class RssItemDtoQuery : DbContextOwningQueryBase, IRssItemDtoQuery
    {

        private ICache<Uri, RssItemEfEntity> persistenceCache;

        /// <summary>
        /// قفل مشترک برای همگام‌سازی کش دائم (persistenceCache) با دیتابیس.
        /// 
        /// این قفل بین چند کلاس به اشتراک گذاشته می‌شود:
        /// - در RssItemRepository با write lock (برای نوشتن در کش دائم، بعد از SaveChanges).
        /// - در این کلاس و RssItemWithDtoQuery با read lock (برای خواندن از کش دائم).
        /// 
        /// هدف: جلوگیری از خواندن نسخه‌ی قدیمی کش، در طول پنجره‌ی بین commit دیتابیس و 
        /// به‌روزرسانی کش.
        /// </summary>
        private ReaderWriterLockSlim sharedPersistenceCacheLock;

        private RssItemDtoEfEntityMapper rssItemDtoMapper;

        private RssItemEfEntityExpressionFactory expressionFactory;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemDtoQuery(RssDbContext dbContext, ICache<Uri, RssItemEfEntity> persistenceCache, 
            ReaderWriterLockSlim sharedPersistenceCacheLock, RssItemDtoEfEntityMapper rssItemDtoMapper, 
            RssItemEfEntityExpressionFactory expressionFactory) : base(dbContext)
        {
            this.persistenceCache = persistenceCache;
            this.sharedPersistenceCacheLock = sharedPersistenceCacheLock;
            this.rssItemDtoMapper = rssItemDtoMapper;
            this.expressionFactory = expressionFactory;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public bool IsRssItemExist(string rssItemId)
        {
            // لازم به جستجو در کش نیست .
            return this.FindByKeyEntity<string, RssItemEfEntity>(rssItemId) != null;
        }


        public RssItemDto FindByKey(string rssItemId)
        {
            /// ⚠️ قفل خواندن: چون این متد با قفل نوشتنِ RssItemRepository مشترک است، 
            /// در طول SaveChanges بلاک می‌شود — و بعد از آن، نسخه‌ی جدید را می‌خواند.
            this.sharedPersistenceCacheLock.EnterReadLock();
            try
            {
                RssItemDto rssItemDto = this.GetRssItemDtoFromCache(rssItemId);
                return (rssItemDto != null) ? rssItemDto : this.GetRssItemDtoFromDatabase(rssItemId);
            }
            finally { this.sharedPersistenceCacheLock.ExitReadLock(); }
        }


        /// <summary>
        /// پیدا کردن مجموعه ای از RssItemDto توسط شناسه ی فید 
        /// 
        /// ⚠️ قفل خواندن: چون این متد با قفل نوشتنِ RssItemRepository مشترک است، 
        /// در طول SaveChanges بلاک می‌شود — و بعد از آن، نسخه‌ی جدید را می‌خواند.
        /// </summary>
        /// <param name="rssFeedId">شناسه ی فید در Rss</param>
        /// <returns></returns>
        public IEnumerable<RssItemDto> FindByFeedId(string rssFeedId)
        {
            /// <para>
            ///⚠️ قاعده: به ازای هر فراخوانی این متد، فقط یک بار به کش رجوع می‌شود.
            /// 
            /// دلیل: در این متد، دو کار مستقل از کش انجام می‌شود:
            ///   ۱. استخراج DTOهای کش‌شده (cachedRssItemDtos)
            ///   ۲. استخراج کلیدهای کش (cacheKeys)، تا از DB فیلتر شوند
            /// 
            /// اگر این دو عملیات در دو دفعه‌ی مجزا انجام شوند، نخ نویسنده می‌تواند بین 
            /// آن‌ها کش را تغییر دهد و باعث ناسازگاری شود. مثلاً یک آیتم در عملیات اول 
            /// باشد ولی در عملیات دوم نباشد (یا برعکس) → آن آیتم در نتیجه گم می‌شود 
            /// (چون هم در cachedDtos حذف شده و هم از DB فیلتر می‌شود).
            /// 
            /// بنابراین، یک snapshot واحد گرفته می‌شود و به هر دو متد پاس داده می‌شود.
            /// </para>
            /// 
            /// <para>
            /// <b>⚠️ چرا قفل خواندن (Read Lock) کل متد را می‌پوشاند؟</b>
            /// 
            /// دو دلیل:
            /// ۱. <b>سازگاری بین چند snapshot از cache:</b> حتی با یک snapshot واحد، 
            ///    اگر قفل نداشته باشیم، بین خواندن snapshot و DB query، نخ نویسنده 
            ///    می‌تواند کش را به‌روز کند و نتیجه‌ی نهایی (cached + DB) ناسازگار شود.
            /// ۲. <b>همگام‌سازی با Repository:</b> این قفل با قفلِ نوشتنِ 
            ///    <c>RssItemRepository</c> مشترک است. پس در طول SaveChanges (که نویسنده 
            ///    write lock دارد)، این متد بلاک می‌شود — و بعد از آزاد شدن، نسخه‌ی 
            ///    جدید کش را می‌خواند.
            /// 
            /// قفل خواندن (نه نوشتن): چون متد فقط می‌خواند و می‌تواند چند خواننده‌ی 
            /// همزمان داشته باشد. ولی نویسنده‌ها بلاک می‌شوند.
            /// </para>
            this.sharedPersistenceCacheLock.EnterReadLock();
            try
            {
                IReadOnlyDictionary<Uri, RssItemEfEntity> cacheSnapshot = this.persistenceCache.GetAllKeyValuesCopy();
                IList<RssItemDto> cachedRssItemDtos = this.GetRssItemDtosByFeedIdFromCache(cacheSnapshot, rssFeedId) ?? new List<RssItemDto>();
                IList<RssItemDto> databaseRssItemDtos = this.GetUncachedRssItemDtosByFeedIdFromDatabase(cacheSnapshot, rssFeedId) ?? new List<RssItemDto>();
                return cachedRssItemDtos.Concat(databaseRssItemDtos)?.ToList();
            }
            finally { this.sharedPersistenceCacheLock.ExitReadLock(); }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        private RssItemDto GetRssItemDtoFromCache(string rssItemId)
        {
            Uri rssItemUriKey = new Uri(rssItemId);
            return (this.persistenceCache.TryGetValue(rssItemUriKey, out RssItemEfEntity rssItemEfEntity) == true) ?
                this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity, isUnsaved: false) : null;
        }


        /// <summary>
        /// استخراج DTO های کش‌شده بر اساس FeedId، از یک snapshot از پیش گرفته‌شده.
        /// 
        /// ⚠️ این متد، خودش به کش رجوع نمی‌کند — snapshot از بیرون پاس داده می‌شود.
        /// دلیل: در FindByFeedId، یک snapshot واحد برای چند مصرف (cachedDtos و cacheKeys) لازم است
        /// تا از ناسازگاری بین آن‌ها جلوگیری شود.
        /// </summary>
        private IList<RssItemDto> GetRssItemDtosByFeedIdFromCache(IReadOnlyDictionary<Uri, RssItemEfEntity> cacheSnapshot, string rssFeedId)
        {
            return cacheSnapshot?.Values
                ?.Where(this.expressionFactory.FuncByFeedId(rssFeedId))
                ?.Select(rssItemEfEntity => this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity, isUnsaved: false))
                ?.ToList();
        }


        private RssItemDto GetRssItemDtoFromDatabase(string rssItemId)
        {
            RssItemEfEntity rssItemEfEntity = this.FindByKeyEntity<string, RssItemEfEntity>(rssItemId);
            if (rssItemEfEntity == null)
                return null;
            return this.rssItemDtoMapper.ToRssItemDto(rssItemEfEntity, isUnsaved: false);
        }


        /// <summary>
        /// استخراج DTO های ذخیره‌نشده در کش، از دیتابیس.
        /// 
        /// ⚠️ این متد، خودش به کش رجوع نمی‌کند — snapshot از بیرون پاس داده می‌شود.
        /// دلیل: همانند GetRssItemDtosByFeedIdFromSnapshot، برای سازگاری با آن.
        /// </summary>
        private IList<RssItemDto> GetUncachedRssItemDtosByFeedIdFromDatabase(
            IReadOnlyDictionary<Uri, RssItemEfEntity> cacheSnapshot, string rssFeedId)
        {
            IList<string> cacheKeys = this.GetCacheKeysByFeedId(cacheSnapshot, rssFeedId);

            IQueryable<RssItemEfEntity> rssItemEfEntityQuery = this.FindByAttributeEntity<RssItemEfEntity>(
                this.expressionFactory.ExpressionByFeedId(rssFeedId));
            if (cacheKeys != null && cacheKeys.Count > 0)
            {
                rssItemEfEntityQuery = rssItemEfEntityQuery.Where(rssItemEfEntity => cacheKeys.Contains(rssItemEfEntity.Id) == false);
            }

            return rssItemEfEntityQuery
                .ToList()
                .Select(e => this.rssItemDtoMapper.ToRssItemDto(e, isUnsaved: false))
                .ToList();
        }


        /// <summary>
        /// استخراج کلیدهای کش‌شده بر اساس FeedId، از یک snapshot از پیش گرفته‌شده.
        /// 
        /// ⚠️ این متد، خودش به کش رجوع نمی‌کند — snapshot از بیرون پاس داده می‌شود.
        /// دلیل: همانند GetRssItemDtosByFeedIdFromSnapshot، برای سازگاری با آن.
        /// </summary>
        private IList<string> GetCacheKeysByFeedId(IReadOnlyDictionary<Uri, RssItemEfEntity> cacheSnapshot, string rssFeedId)
        {
            return cacheSnapshot?.Values
                ?.Where(rssItemEfEntity => rssItemEfEntity.RssFeedEfEntityId == rssFeedId)
                ?.Select(rssItemEfEntity => rssItemEfEntity.Id)
                ?.ToList();
        }

    }
}
