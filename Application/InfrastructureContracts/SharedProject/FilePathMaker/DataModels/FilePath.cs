namespace Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels
{
    public class FilePath
    {

        public string FolderPath { get; set; }

        public string FileName { get; set; }


        /////////////////////////////////////////////////////////////////////////////

        public FilePath()
        {

        }


        public FilePath(string folderPath, string fileName)
        {
            this.FolderPath = folderPath;
            this.FileName = fileName;
        }

    }
}
