using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;

namespace Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Factory.Interfaces
{
    public interface IRssItemRepositoryFactory
    {

        IRssItemRepository Create(IUnitOfWork unitOfWork);

    }
}
