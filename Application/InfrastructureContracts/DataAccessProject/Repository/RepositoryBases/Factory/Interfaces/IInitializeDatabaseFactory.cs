using Application.InfrastructureContracts.DataAccessProject.Repository.RepositoryBases.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Interfaces;

namespace Application.InfrastructureContracts.DataAccessProject.Repository.RepositoryBases.Factory.Interfaces
{
    public interface IInitializeDatabaseFactory
    {

        IInitializeDatabase Create(IUnitOfWork unitOfWork);

    }
}
