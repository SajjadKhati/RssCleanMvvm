using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssFeedXmlParsingConfigQuery.Interface;
using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;
using Application.InfrastructureContracts.JsonProject.Json.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Infra.DataAccess.AppQueryService.RssFeedXmlParsingConfigQueryService.Class;
using Infra.DataAccess.DataAccessBaseServices.DatabaseInitializationService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.JsonService.JsonServices.Class;
using Microsoft.EntityFrameworkCore;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class
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


        public async Task<bool> InitializeAppAsync(string connectionString)
        {
            return await Task.Run(() => this.InitializeApp(connectionString));
        }


        public async Task<IReadOnlyDictionary<Uri, RssFeedXmlParsingConfig>> FinAllRssFeedXmlParsingConfigAsync(string connectionString)
        {
            return await Task.Run(() => this.FinAllRssFeedXmlParsingConfig(connectionString));
        }


        ///////////////////////////////////////////////////////////////////////


        private string GetConnectionString(string connectionStringJsonFullFilePath)
        {
            return this.jsonFile.ReadJson<string>(connectionStringJsonFullFilePath);
        }


        private bool InitializeApp(string connectionString)
        {
            DbContext dbContext = new RssDbContext(connectionString);
            using IInitializeDatabase initializeDatabase = new InitializeDatabase(dbContext);
            return initializeDatabase.EnsureDatabaseCreated();
        }


        private IReadOnlyDictionary<Uri, RssFeedXmlParsingConfig> FinAllRssFeedXmlParsingConfig(string connectionString)
        {
            RssDbContext rssDbContext = new RssDbContext(connectionString);
            RssFeedXmlParsingConfigEfEntityMapper configEfEntityMapper = new RssFeedXmlParsingConfigEfEntityMapper();
            using IRssFeedXmlParsingConfigQuery rssFeedXmlParsingConfigQuery = new RssFeedXmlParsingConfigQuery(rssDbContext, configEfEntityMapper);
            return rssFeedXmlParsingConfigQuery.FindAll();
        }

    }
}
