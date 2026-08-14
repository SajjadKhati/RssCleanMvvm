using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.Interfaces;
using Infra.SharedService.ImageSettingsMapperService.Class;

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

        private IFilePathMaker filePathMaker;

        private ImageSettingsMapper imageSettingsMapper;


        /////////////////////////////////////////////////////////////////////////////


        public ImageFileSaverCoordinator(IImageFileSaver imageFileSaver, IFilePathMaker filePathMaker, ImageSettingsMapper imageSettingsMapper)
        {
            this.imageFileSaver = imageFileSaver;
            this.filePathMaker = filePathMaker;
            this.imageSettingsMapper = imageSettingsMapper;
        }


        /////////////////////////////////////////////////////////////////////////////


        public DestinationPath Save8BitImageToFile(byte[,,] image, ImageSettings<ImageInfo> imageInfoSettings, FilePath filePath)
        {
            DestinationPath destinationPath = this.filePathMaker.CreateFullImageFilePathWithSafeFileName(filePath, imageInfoSettings.Format);
            try
            {
                var imageFileSaveSettings = new ImageFileSaveSettings
                {
                    ImageFileSettings = this.imageSettingsMapper.MappToImageDataSettings(imageInfoSettings, image),
                    FullFilePath = destinationPath.FullFilePath
                };
                this.imageFileSaver.Save8BitImageToFile(imageFileSaveSettings);
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
