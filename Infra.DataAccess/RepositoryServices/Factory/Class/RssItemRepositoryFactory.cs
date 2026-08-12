using Application.InfrastructureContracts.CacheProject.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.RepositoryServices.Class.ProgrameRepositories;
using Infra.DataAccess.UnitOfWorkService.InternalInterface;

namespace Infra.DataAccess.RepositoryServices.Factory.Class
{
    public class RssItemRepositoryFactory : IRssItemRepositoryFactory
    {

        private ICache<Uri, RssItemEfEntity> persistanceCache;

        private RssItemEfEntityMapper efEntityMapper;

        private IImageFileSaverCoordinator imageFileSaverCoordinator;

        private IImageFileReader imageFileReader;


        /////////////////////////////////////////////////////////////////////////


        public RssItemRepositoryFactory(RssItemEfEntityMapper efEntityMapper, ICache<Uri, RssItemEfEntity> persistanceCache,
            IImageFileSaverCoordinator imageFileSaverCoordinator, IImageFileReader imageFileReader)
        {
            this.persistanceCache = persistanceCache;
            this.efEntityMapper = efEntityMapper;
            this.imageFileSaverCoordinator = imageFileSaverCoordinator;
            this.imageFileReader = imageFileReader;
        }


        /////////////////////////////////////////////////////////////////////////


        public IRssItemRepository Create(IUnitOfWork unitOfWork)
        {
            if(unitOfWork is not IDbContextProvider dbContextProvider)
                throw new ArgumentException($"شی {nameof(unitOfWork)} ، باید رابط IDbContextProvider را هم پیاده سازی کرده باشد .");
            if(dbContextProvider.DbContext is not RssDbContext rssDbContext)
                throw new ArgumentException($"نوع {nameof(dbContextProvider.DbContext)} ، باید از نوع RssDbContext باشد .");

            return new RssItemRepository(rssDbContext, this.persistanceCache, this.efEntityMapper, this.imageFileSaverCoordinator, this.imageFileReader);
        }


    }
}
