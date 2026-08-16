using Application.InfrastructureContracts.CacheProject.Cache.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.AppRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Infra.DataAccess.AppRepositoryService.Class;
using Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.InternalInterface;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;

namespace Infra.DataAccess.AppRepositoryService.Factory.Class
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
            /// اینجا باید همون شیِ DbContext ای که در شی UnitOfWork استفاده شده را برای شی Repository مان بکار ببریم .
            /// چون هر دو باید روی یک شی کار کنند تا تغییرات ، اِعمال بشه .
            if (unitOfWork is not IDbContextProvider dbContextProvider)
                throw new ArgumentException($"شی {nameof(unitOfWork)} ، باید رابط IDbContextProvider را هم پیاده سازی کرده باشد .");
            if(dbContextProvider.DbContext is not RssDbContext rssDbContext)
                throw new ArgumentException($"نوع {nameof(dbContextProvider.DbContext)} ، باید از نوع RssDbContext باشد .");

            return new RssItemRepository(rssDbContext, this.persistanceCache, this.efEntityMapper, this.imageFileSaverCoordinator, this.imageFileReader);
        }


    }
}
