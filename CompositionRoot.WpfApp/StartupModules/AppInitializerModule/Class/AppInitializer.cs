using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;
using Application.InfrastructureContracts.JsonProject.Json.Interface;
using Infra.DataAccess.DataAccessBaseServices.DatabaseInitializationService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.JsonService.JsonServices.Class;
using Microsoft.EntityFrameworkCore;

namespace CompositionRoot.WpfApp.StartupModules.AppInitializerModule.Class
{
    public class AppInitializer
    {

        private IJsonFile jsonFile;


        ///////////////////////////////////////////////////////////////////////


        public AppInitializer()
        {
            this.jsonFile = new JsonFile();
        }


        ///////////////////////////////////////////////////////////////////////


        public async Task<string> GetConnectionStringAsync(string connectionStringJsonFullFilePath)
        {
            return await Task.Run(() => this.GetConnectionString(connectionStringJsonFullFilePath));
        }


        public async Task InitializeAppAsync(string connectionString)
        {
            await Task.Run(() => this.InitializeApp(connectionString));
        }


        private string GetConnectionString(string connectionStringJsonFullFilePath)
        {
            return this.jsonFile.ReadJson<string>(connectionStringJsonFullFilePath);
        }


        private void InitializeApp(string connectionString)
        {
            DbContext dbContext = new RssDbContext(connectionString);
            using IInitializeDatabase initializeDatabase = new InitializeDatabase(dbContext);
            initializeDatabase.EnsureDatabaseCreated();
        }

    }
}
