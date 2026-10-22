using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.RssItemDtoFactory.Interface;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.SharedProject.AppFolderPath.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.DraftRssItemDtoFactoryOrch.Class
{
    public class DraftRssItemDtoFactoryOrchestrator : IDraftRssItemDtoFactoryOrchestrator
    {

        private IRssItemDtoFactory rssItemDtoFactory;

        private IImageSettingsReader imageSettingsReader;

        private ITempImageFolderPath tempImageFolderPath;


        /////////////////////////////////////////////////////////////////////////


        public DraftRssItemDtoFactoryOrchestrator(IRssItemDtoFactory rssItemDtoFactory, 
            IImageSettingsReader imageSettingsReader, ITempImageFolderPath tempImageFolderPath)
        {
            this.rssItemDtoFactory = rssItemDtoFactory;
            this.imageSettingsReader = imageSettingsReader;
            this.tempImageFolderPath = tempImageFolderPath;
        }


        /////////////////////////////////////////////////////////////////////////


        public RssItemDto CreateRssItemDtoWithSaveImage(RssItem rssItem, bool isUnsaved)
        {
            ImageSettings<ImageInfo> imageSettings = this.imageSettingsReader.GetOrDefault(rssItem.Id, rssItem.Argb8BitImage);
            ImageSaveOptions imageSaveOptions = new ImageSaveOptions(imageSettings, this.tempImageFolderPath.FolderPath);
            return this.rssItemDtoFactory.CreateRssItemDto(rssItem, imageSaveOptions, isUnsaved);
        }

    }
}
