using Application.InfrastructureContracts.CacheProject.AppDraftCacheManager.DraftCacheWriter.Interface;
using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Infra.DataAccess.AppRepositoryService.RssItemRepositoryService.Class;
using Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.InternalInterface;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.ExpressionFactoryService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface;
using Infra.DataAccess.RssItemReconstructorService.Interface;

namespace Infra.DataAccess.AppRepositoryService.RssItemRepositoryService.Factory.Class
{
    public class RssItemRepositoryFactory : IRssItemRepositoryFactory
    {

        private ICache<Uri, RssItemEfEntity> persistenceCache;

        private ReaderWriterLockSlim sharedPersistenceCacheLock;

        private IDraftCacheWriter draftCacheWriter;

        private RssItemEfEntityMapper efEntityMapper;

        private IRssItemReconstructor rssItemReconstructor;

        private IRssItemEfEntityFactory rssItemEfEntityFactory;

        private RssItemEfEntityExpressionFactory expressionFactory;


        /////////////////////////////////////////////////////////////////////////


        public RssItemRepositoryFactory(RssItemEfEntityMapper efEntityMapper, ICache<Uri, RssItemEfEntity> persistenceCache,
            ReaderWriterLockSlim sharedPersistenceCacheLock, IDraftCacheWriter draftCacheWriter, IRssItemReconstructor rssItemReconstructor, 
            IRssItemEfEntityFactory rssItemEfEntityFactory, RssItemEfEntityExpressionFactory expressionFactory)
        {
            this.persistenceCache = persistenceCache;
            this.sharedPersistenceCacheLock = sharedPersistenceCacheLock;
            this.draftCacheWriter = draftCacheWriter;
            this.efEntityMapper = efEntityMapper;
            this.rssItemReconstructor = rssItemReconstructor;
            this.rssItemEfEntityFactory = rssItemEfEntityFactory;
            this.expressionFactory = expressionFactory;
        }


        /////////////////////////////////////////////////////////////////////////


        public IRssItemRepository Create(IUnitOfWork unitOfWork)
        {
            /// اینجا باید همون شیِ DbContext ای که در شی UnitOfWork استفاده شده را برای شی Repository مان بکار ببریم .
            /// چون هر دو باید روی یک شی کار کنند تا تغییرات ، اِعمال بشه .
            if (unitOfWork is not IDbContextProvider dbContextProvider)
                throw new ArgumentException($"شی {nameof(unitOfWork)} ، باید رابط IDbContextProvider را هم پیاده سازی کرده باشد .");
            if(dbContextProvider.DbContext is not RssDbContext rssDbContext)
                throw new ArgumentException($"نوع {nameof(dbContextProvider.DbContext)} ، باید از نوع RssDbContext باشد .");

            return new RssItemRepository(rssDbContext, this.persistenceCache, this.sharedPersistenceCacheLock, this.draftCacheWriter, 
                this.efEntityMapper, this.rssItemReconstructor, this.rssItemEfEntityFactory, this.expressionFactory);
        }

    }
}
