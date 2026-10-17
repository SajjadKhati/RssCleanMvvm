using Application.InfrastructureContracts.SharedProject.AppFolderPath.Interface;

namespace CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class
{
    public class AppPaths : ITempImageFolderPath, IPersistentImageFolderPath
    {

        private string appName = "RssCleanMvvm";

        private string userTempFolderPath = Path.GetTempPath();

        private string userLocalFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);


        //////////////////////////////////////////////////////////////


        public string TempImagesFolderPath { get; }

        public string PersistentImagesFolderPath { get; }

        string ITempImageFolderPath.FolderPath => this.TempImagesFolderPath;

        string IPersistentImageFolderPath.FolderPath => this.PersistentImagesFolderPath;


        //////////////////////////////////////////////////////////////


        public AppPaths()
        {
            this.TempImagesFolderPath = Path.Combine(this.userTempFolderPath, this.appName, "TempImages");
            this.PersistentImagesFolderPath = Path.Combine(this.userLocalFolderPath, this.appName, "PermanentImages");
        }

    }
}
