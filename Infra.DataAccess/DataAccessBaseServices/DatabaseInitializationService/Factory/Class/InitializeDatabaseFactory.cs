using Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.InternalInterface;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Infra.DataAccess.DataAccessBaseServices.DatabaseInitializationService.Class;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Factory.Interfaces;

namespace Infra.DataAccess.DataAccessBaseServices.DatabaseInitializationService.Factory.Class
{
    public class InitializeDatabaseFactory : IInitializeDatabaseFactory
    {

        public IInitializeDatabase Create(IUnitOfWork unitOfWork)
        {
            if (unitOfWork is not IDbContextProvider dbContextProvider)
                throw new ArgumentException($"شی {nameof(unitOfWork)} ، باید رابط IDbContextProvider را هم پیاده سازی کرده باشد .");

            return new InitializeDatabase(dbContextProvider.DbContext);
        }

    }
}
