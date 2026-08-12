using Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Interfaces;

namespace Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.Factory.Interfaces
{
    public interface IRssItemRepositoryFactory
    {

        IRssItemRepository Create(IUnitOfWork unitOfWork);

    }
}
