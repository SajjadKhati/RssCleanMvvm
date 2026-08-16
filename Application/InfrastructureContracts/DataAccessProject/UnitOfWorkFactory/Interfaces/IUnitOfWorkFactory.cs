using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;

namespace Application.InfrastructureContracts.DataAccessProject.UnitOfWorkFactory.Interfaces
{
    public interface IUnitOfWorkFactory
    {

        IUnitOfWork Create(bool shouldUseLazyLoading = false);

    }
}
