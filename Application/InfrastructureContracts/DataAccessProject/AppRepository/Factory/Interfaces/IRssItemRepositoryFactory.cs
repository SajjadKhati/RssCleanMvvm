using Application.InfrastructureContracts.DataAccessProject.AppRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;

namespace Application.InfrastructureContracts.DataAccessProject.AppRepository.Factory.Interfaces
{
    public interface IRssItemRepositoryFactory
    {

        IRssItemRepository Create(IUnitOfWork unitOfWork);

    }
}
