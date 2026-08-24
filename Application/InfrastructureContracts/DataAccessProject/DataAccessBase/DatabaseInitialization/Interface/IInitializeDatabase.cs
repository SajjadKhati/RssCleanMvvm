namespace Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface
{
    public interface IInitializeDatabase : IDisposable
    {

        bool EnsureDatabaseCreated();

    }
}
