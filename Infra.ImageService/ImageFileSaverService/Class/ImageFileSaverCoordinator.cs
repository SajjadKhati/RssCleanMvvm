using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;
using Infra.SharedService.FilePathMakerService.Class;

namespace Infra.ImageService.ImageFileSaverService.Class
{
    /// <summary>
    /// کلاس ImageFileSaverCoordinator ، از لحاظ اصل وارونگی وابستگی ، سطح پایین تر از کلاس ImageFileSaver محسوب میشه . 
    /// یعنی ImageFileSaverCoordinator ، سیاست را تعیین نمیکنه و وابسته به ImageFileSaver هست و ازش استفاده میکنه . 
    /// و همچنین هماهنگ کننده ی بین سرویس ImageFileSaver و FilePathMaker هست . 
    /// </summary>
    public class ImageFileSaverCoordinator : IImageFileSaverCoordinator
    {

        private IImageFileSaver imageFileSaver;

        private FilePathMaker filePathMaker;


        /////////////////////////////////////////////////////////////////////////////


        public ImageFileSaverCoordinator(IImageFileSaver imageFileSaver, FilePathMaker filePathMaker)
        {
            this.imageFileSaver = imageFileSaver;
            this.filePathMaker = filePathMaker;
        }


        /////////////////////////////////////////////////////////////////////////////


        public DestinationPath Save8BitImageToFile(ImageFileSettings imageFileSettings, FilePath filePath)
        {
            DestinationPath destinationPath = this.filePathMaker.CreateFullImageFilePath(filePath, imageFileSettings.Format);
            try
            {
                var imageFileSpecification = new ImageFileSpecification
                {
                    ImageFileSettings = imageFileSettings,
                    FullFilePath = destinationPath.FullFilePath
                };
                this.imageFileSaver.Save8BitImageToFile(imageFileSpecification);
                return destinationPath;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"عملیات ذخیره سازی فایل شکست خورد. بنابراین شی جاریِ RssItem ذخیره نمی‌شود.", exception);
            }
        }

    }
}
