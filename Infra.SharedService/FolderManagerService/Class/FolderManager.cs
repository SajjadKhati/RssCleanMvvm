using Application.InfrastructureContracts.SharedProject.FolderManager.Interface;

namespace Infra.SharedService.FolderManagerService.Class
{
    public class FolderManager : IFolderManager
    {

        public bool IsFolderExist(string folderPath)
        {
            return Directory.Exists(folderPath);
        }


        public void CreateFolder(string folderPath)
        {
            /// اگر وجود نداشته باشد می‌سازد؛ اگر وجود داشته باشد کاری نمی‌کند .
            Directory.CreateDirectory(folderPath);
        }

    }
}
