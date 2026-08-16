namespace Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        int SaveChanges();

    }
}
