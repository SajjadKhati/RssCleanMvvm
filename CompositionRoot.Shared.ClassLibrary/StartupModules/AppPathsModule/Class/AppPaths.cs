namespace CompositionRoot.Shared.ClassLibrary.StartupModules.AppPathsModule.Class
{
    public class AppPaths
    {

        private string appName = "RssCleanMvvm";

        private string userTempFolderPath = Path.GetTempPath();

        private string userLocalFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);


        //////////////////////////////////////////////////////////////


        public string TempImagesFolderPath { get; }

        public string PermanentImagesFolderPath { get; }


        //////////////////////////////////////////////////////////////


        public AppPaths()
        {
            this.TempImagesFolderPath = Path.Combine(this.userTempFolderPath, this.appName, "TempImages");
            this.PermanentImagesFolderPath = Path.Combine(this.userLocalFolderPath, this.appName, "PermanentImages");
        }

    }
}
