using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces;
using Application.InfrastructureContracts.SharedProject.Hasher.Interface;

namespace Infra.SharedService.FilePathMakerService.Class
{
    public class FilePathMaker : IFilePathMaker
    {

        private IStringHasher stringHasher;


        ///////////////////////////////////////////////////////////////////////////////


        public FilePathMaker(IStringHasher stringHasher)
        {
            this.stringHasher = stringHasher;
        }


        ///////////////////////////////////////////////////////////////////////////////


        public DestinationPath CreateDestinationPathWithHashedFileName(FullFilePath fullFilePath)
        {
            string hashedFileName = this.stringHasher.HashString(fullFilePath.InitialFileName);
            return this.GetDestinationPath(fullFilePath.FolderPath, hashedFileName, fullFilePath.FileExtension);
        }


        ///////////////////////////////////////////////////////////////////////////////


        private DestinationPath GetDestinationPath(string folderPath, string fileName, string fileExtension)
        {
            return new DestinationPath
            {
                FolderPath = folderPath,
                FileName = fileName,
                Extension = fileExtension,
                FullFilePath = Path.Combine(folderPath, $"{fileName}.{fileExtension}")
            };
        }

    }
}
