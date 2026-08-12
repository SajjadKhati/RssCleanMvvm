namespace Application.InfrastructureContracts.DataAccessProject.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        int SaveChanges();

    }
}
