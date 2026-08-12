using Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Interfaces;

namespace Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Factory.Interfaces
{
    public interface IUnitOfWorkFactory
    {

        IUnitOfWork Create(bool shouldUseLazyLoading);

    }
}
