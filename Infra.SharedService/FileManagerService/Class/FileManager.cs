using Application.InfrastructureContracts.SharedProject.FileManager.Interface;

namespace Infra.SharedService.FileManagerService.Class
{
    public class FileManager : IFileManager
    {

        public void DeleteFile(string fullFilePath)
        {
            File.Delete(fullFilePath);
        }

    }
}
