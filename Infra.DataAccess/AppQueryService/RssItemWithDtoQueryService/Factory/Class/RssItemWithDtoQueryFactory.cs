using Application.InfrastructureContracts.CacheProject.CacheBase.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemWithDtoQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemWithDtoQuery.Interface;
using Infra.DataAccess.AppQueryService.RssItemWithDtoQueryService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.RssItemReconstructorService.Interface;
using Infra.DataAccess.EfEntityMapperService.Class;

namespace Infra.DataAccess.AppQueryService.RssItemWithDtoQueryService.Factory.Class
{
    public class RssItemWithDtoQueryFactory : IRssItemWithDtoQueryFactory
    {

        private string connectionString;

        private ICache<Uri, RssItemEfEntity> persistenceCache;

        private ReaderWriterLockSlim sharedPersistenceCacheLock;

        private IRssItemReconstructor rssItemReconstructor;

        private RssItemDtoEfEntityMapper RssItemDtoEfEntityMapper;


        ////////////////////////////////////////////////////////////////////////////////////////////


        public RssItemWithDtoQueryFactory(string connectionString, ICache<Uri, RssItemEfEntity> persistenceCache,
            ReaderWriterLockSlim sharedPersistenceCacheLock, IRssItemReconstructor rssItemReconstructor, 
            RssItemDtoEfEntityMapper RssItemDtoEfEntityMapper)
        {
            this.connectionString = connectionString;
            this.persistenceCache = persistenceCache;
            this.sharedPersistenceCacheLock = sharedPersistenceCacheLock;
            this.rssItemReconstructor = rssItemReconstructor;
            this.RssItemDtoEfEntityMapper = RssItemDtoEfEntityMapper;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////


        public IRssItemWithDtoQuery Create()
        {
            return new RssItemWithDtoQuery(new RssDbContext(this.connectionString), this.persistenceCache, this.sharedPersistenceCacheLock,
                this.rssItemReconstructor, this.RssItemDtoEfEntityMapper);
        }

    }
}
