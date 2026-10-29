using Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using ImageMagick;
using Infra.ImageService.InfraImageCreatorService.Class;
using Infra.ImageService.MappingService.Class;

namespace Infra.ImageService.ImageDataExtractorService.Class
{
    public class ImageDataExtractor : ImageInfoExtractor, IImageDataExtractor
    {

        private InfraImageCreator infraImageCreator;


        ////////////////////////////////////////////////////
        

        public ImageDataExtractor(InfraImageCreator infraImageCreator, ImageInfoMemberMapper imageInfoMemberMapper)
            : base(imageInfoMemberMapper)
        {
            this.infraImageCreator = infraImageCreator;
        }


        ////////////////////////////////////////////////////


        public ImageData<byte> Get8BitImageData(byte[] rawImage)
        {
            byte[,,] imageData = this.Get8BitImage(rawImage);
            var imageInfo = this.GetImageInfo(rawImage);

            return new ImageData<byte>(imageInfo, imageData);
        }


        public byte[,,] Get8BitImage(byte[] rawImage)
        {
            using var image = new MagickImage(rawImage);
            int width = (int)image.Width;
            int height = (int)image.Height;
            int channelCount = (int)image.ChannelCount;

            using var pixels = image.GetPixels();
            byte[,,] imageData = this.infraImageCreator.GetImageDataArray(width, height, channelCount, pixels);
            return imageData;
        }


        /// <summary>
        /// خواندن تصویر از byte[] خام و برگرداندن byte[,,] با ترتیب ARGB.
        /// برای خواندن از فایل استفاده می‌شود (نه مسیر دانلود).
        /// </summary>
        /// <param name="rawImage"></param>
        /// <returns></returns>
        public byte[,,] GetArgb8BitImage(byte[] rawImage)
        {
            using var image = new MagickImage(rawImage);
            int width = (int)image.Width;
            int height = (int)image.Height;
            int channelCount = (int)image.ChannelCount;

            using var pixels = image.GetPixels();
            return this.infraImageCreator.GetArgb8BitImageDataArray(width, height, channelCount, pixels);
        }

    }
}
