using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;

namespace Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces
{
    public interface IFilePathMaker
    {

        DestinationPath CreateFullImageFilePathWithSafeFileName(FilePath filePath, MagickFormat magickFormat);

        DestinationPath CreateFullImageFilePath(FilePath filePath, MagickFormat magickFormat);

    }
}
