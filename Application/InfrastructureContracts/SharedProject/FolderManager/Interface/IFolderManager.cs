namespace Application.InfrastructureContracts.SharedProject.FolderManager.Interface
{
    public interface IFolderManager
    {

        bool IsFolderExist(string folderPath);

        void CreateFolder(string folderPath);

    }
}
