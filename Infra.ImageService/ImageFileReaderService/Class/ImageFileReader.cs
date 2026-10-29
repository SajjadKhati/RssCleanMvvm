using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.ImageProject.ImageFileReader.Interfaces;

namespace Infra.ImageService.ImageFileReaderService.Class
{
    public class ImageFileReader : IImageFileReader
    {

        private IImageDataExtractor imageDataExtractor;


        //////////////////////////////////////////////////////////////
        

        public ImageFileReader(IImageDataExtractor imageDataExtractor)
        {
            this.imageDataExtractor = imageDataExtractor;
        }


        //////////////////////////////////////////////////////////////


        public byte[,,] ReadArgb8BitImageFromFile(string fullFilePath)
        {
            byte[] rawImage =  File.ReadAllBytes(fullFilePath);
            return this.imageDataExtractor.GetArgb8BitImage(rawImage);
        }

    }
}
