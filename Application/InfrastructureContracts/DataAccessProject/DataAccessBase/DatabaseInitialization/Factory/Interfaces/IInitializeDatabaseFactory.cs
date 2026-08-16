using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;

namespace Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Factory.Interfaces
{
    public interface IInitializeDatabaseFactory
    {

        IInitializeDatabase Create(IUnitOfWork unitOfWork);

    }
}
