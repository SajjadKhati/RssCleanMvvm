using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;

namespace Application.InfrastructureContracts.ImageProject.ImageDataExtractor.Interfaces
{
    public interface IImageDataExtractor : IImageInfoExtractor
    {

        ImageData<byte> Get8BitImageData(byte[] rawImage);

        byte[,,] Get8BitImage(byte[] rawImage);

        /// <summary>
        /// خواندن تصویر از byte[] خام و برگرداندن byte[,,] با ترتیب ARGB.
        /// برای خواندن از فایل استفاده می‌شود (نه مسیر دانلود).
        /// </summary>
        /// <param name="rawImage"></param>
        /// <returns></returns>
        byte[,,] GetArgb8BitImage(byte[] rawImage);

    }
}
