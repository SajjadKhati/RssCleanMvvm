using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;

namespace Infra.DataAccess.DataAccessBaseServices.DatabaseInitializationService.Class
{
    public class InitializeDatabase : IInitializeDatabase
    {

        private DbContext dbContext;


        /////////////////////////////////////////////


        public InitializeDatabase(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        /////////////////////////////////////////////


        public bool EnsureDatabaseCreated()
        {
            this.dbContext.Database.Migrate();
            bool isDatabaseExist =  this.dbContext.Database.GetService<IRelationalDatabaseCreator>().Exists();
            return isDatabaseExist;
        }


        public void Dispose()
        {
            this.dbContext.Dispose();
        }

    }
}
