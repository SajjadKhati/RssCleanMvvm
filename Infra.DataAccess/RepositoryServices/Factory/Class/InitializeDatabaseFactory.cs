using Infra.DataAccess.UnitOfWorkService.InternalInterface;
using Application.InfrastructureContracts.DataAccessProject.Repository.RepositoryBases.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.Repository.RepositoryBases.Factory.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Interfaces;
using Infra.DataAccess.RepositoryServices.Class.RepositoryBases;

namespace Infra.DataAccess.RepositoryServices.Factory.Class
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
