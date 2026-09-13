namespace Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels
{
    public class FullFilePath
    {

        public string FolderPath { get; set; }

        public string InitialFileName { get; set; }

        public string FileExtension { get; set; }


        /////////////////////////////////////////////////////////////////////////////

        public FullFilePath()
        {

        }


        public FullFilePath(string folderPath, string initialFileName, string fileExtension)
        {
            this.FolderPath = folderPath;
            this.InitialFileName = initialFileName;
            this.FileExtension = fileExtension;
        }
    }
}
