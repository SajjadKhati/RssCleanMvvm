using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;

namespace Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces
{
    public interface IFilePathMaker
    {
        DestinationPath CreateDestinationPathWithHashedFileName(FullFilePath fullFilePath);
    }
}
