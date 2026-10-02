using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.RssItemDtoFactory.Interface;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageSettings.ImageSettingsProvider.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.RssItemDtoFactory.Interface;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.UseCases.Shared.RssItemDtoFactory.Class
{
    public class RssItemDtoFactoryOrchestrator : IRssItemDtoFactoryOrchestrator
    {

        private IRssItemDtoFactory rssItemDtoFactory;

        private IImageSettingsReader imageSettingsReader;

        private readonly string draftImageFolderPath;


        /////////////////////////////////////////////////////////////////////////


        public RssItemDtoFactoryOrchestrator(IRssItemDtoFactory rssItemDtoFactory, 
            IImageSettingsReader imageSettingsReader, string draftImageFolderPath)
        {
            this.rssItemDtoFactory = rssItemDtoFactory;
            this.imageSettingsReader = imageSettingsReader;
            this.draftImageFolderPath = draftImageFolderPath;
        }


        /////////////////////////////////////////////////////////////////////////


        public RssItemDto CreateRssItemDto(RssItem rssItem)
        {
            ImageSettings<ImageInfo> imageSettings = this.imageSettingsReader.GetOrDefault(rssItem.Id, rssItem.Argb8BitImage);
            ImageSaveOptions imageSaveOptions = new ImageSaveOptions(imageSettings, this.draftImageFolderPath);
            return this.rssItemDtoFactory.CreateRssItemDto(rssItem, imageSaveOptions);
        }

    }
}
