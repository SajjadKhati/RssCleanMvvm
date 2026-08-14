using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Enum;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces;

namespace Infra.SharedService.FilePathMakerService.Class
{
    public class FilePathMaker : IFilePathMaker
    {

        public DestinationPath CreateFullImageFilePathWithSafeFileName(FilePath filePath, MagickFormat magickFormat)
        {
            string fileExtension = this.GetFileExtension(magickFormat);
            string safeFileName = this.EncodeFileName(filePath.FileName);
            return this.GetDestinationPath(filePath.FolderPath, safeFileName, fileExtension);
        }


        public DestinationPath CreateFullImageFilePath(FilePath filePath, MagickFormat magickFormat)
        {
            string fileExtension = this.GetFileExtension(magickFormat);
            return this.GetDestinationPath(filePath.FolderPath, filePath.FileName, fileExtension);
        }


        public string EncodeFileName(string uri)
        {
            return Uri.EscapeDataString(uri);
        }


        public string DecodeFileName(string uri)
        {
            return Uri.UnescapeDataString(uri);
        }


        ////////////////////////////////

        private string GetFileExtension(MagickFormat magickFormat)
        {
            return magickFormat switch
            {
                MagickFormat.Png or MagickFormat.Png00 or MagickFormat.Png8 or MagickFormat.Png24 or MagickFormat.Png32
                    or MagickFormat.Png48 or MagickFormat.Png64 => "png",
                MagickFormat.Jpeg or MagickFormat.Jpg => "jpg",
                MagickFormat.Bmp => "bmp",
                MagickFormat.Tif or MagickFormat.Tiff or MagickFormat.Tiff64 => "tiff",
                _ => throw new FormatException("فرمت نباید در حالت ناشناخته باشد .")
            };
        }


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
