using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.DatabaseInitialization.Interface;
using Application.InfrastructureContracts.JsonProject.Json.Interface;
using Application.InfrastructureContracts.SharedProject.FolderManager.Interface;
using CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class;
using Infra.DataAccess.DataAccessBaseServices.DatabaseInitializationService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.JsonService.JsonServices.Class;
using Infra.SharedService.FolderManagerService.Class;
using Microsoft.EntityFrameworkCore;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.AppInitializerModule.Class
{
    public class AppInitializer
    {

        private IJsonFile jsonFile;

        private IFolderManager folderManager;

        private AppPaths appPaths;


        ///////////////////////////////////////////////////////////////////////


        public AppInitializer(AppPaths appPaths)
        {
            this.jsonFile = new JsonFile();
            this.folderManager = new FolderManager();
            this.appPaths = appPaths;
        }


        ///////////////////////////////////////////////////////////////////////


        public async Task<string> GetConnectionStringAsync(string connectionStringJsonFullFilePath)
        {
            return await Task.Run(() => this.GetConnectionString(connectionStringJsonFullFilePath));
        }


        public async Task<bool> InitializeAppAsync(string connectionString)
        {
            return await Task.Run(() =>
            {
                this.EnsureAppFoldersCreated(new List<string> { this.appPaths.TempImagesFolderPath, this.appPaths.PersistentImagesFolderPath });
                return this.InitializeApp(connectionString);
            });
        }


        ///////////////////////////////////////////////////////////////////////


        private string GetConnectionString(string connectionStringJsonFullFilePath)
        {
            return this.jsonFile.ReadJson<string>(connectionStringJsonFullFilePath);
        }


        private void EnsureAppFoldersCreated(IEnumerable<string> folderPaths)
        {
            foreach (string folderPath in folderPaths)
            {
                if(this.folderManager.IsFolderExist(folderPath) == false)
                    this.folderManager.CreateFolder(folderPath);
            }
        }


        private bool InitializeApp(string connectionString)
        {
            DbContext dbContext = new RssDbContext(connectionString);
            using IInitializeDatabase initializeDatabase = new InitializeDatabase(dbContext);
            return initializeDatabase.EnsureDatabaseCreated();
        }

    }
}
