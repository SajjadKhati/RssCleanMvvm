using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.InfrastructureContracts.DataAccessProject.RssItemDtoFactory.Interface;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Application.InfrastructureContracts.ImageProject.ImageFileSaver.Interfaces;
using Application.InfrastructureContracts.ImageProject.SharedDataModels.Class;
using Application.InfrastructureContracts.SharedProject.FilePathMaker.DataModels;
using Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface;

namespace Infra.DataAccess.RssItemDtoEfEntityFactoryService.Class
{
    public class RssItemDtoEfEntityFactory : IRssItemDtoFactory, IRssItemEfEntityFactory
    {

        private IImageFileSaverCoordinator imageFileSaverCoordinator;

        private IRssItemDtoMapper rssItemDtoMapper;

        private RssItemEfEntityMapper efEntityMapper;


        ////////////////////////////////////////////////////////////////////////


        public RssItemDtoEfEntityFactory(IImageFileSaverCoordinator imageFileSaverCoordinator, IRssItemDtoMapper rssItemDtoMapper,
            RssItemEfEntityMapper efEntityMapper)
        {
            this.imageFileSaverCoordinator = imageFileSaverCoordinator;
            this.rssItemDtoMapper = rssItemDtoMapper;
            this.efEntityMapper = efEntityMapper;
        }


        ////////////////////////////////////////////////////////////////////////


        public RssItemDto CreateRssItemDto(RssItem rssItem, ImageSaveOptions imageSaveOptions)
        {
            DestinationPath draftImageDestinationPath = this.SaveImageToFile(rssItem.Argb8BitImage, imageSaveOptions.FolderPath,
                rssItem.Id.AbsoluteUri, imageSaveOptions.ImageInfoSettings);
            return this.rssItemDtoMapper.ToRssItemDto(rssItem, draftImageDestinationPath.FullFilePath);
        }


        public RssItemEfEntity CreateRssItemEfEntity(RssItem rssItem, ImageSaveOptions imageSaveOptions)
        {
            DestinationPath imageDestinationPath = this.SaveImageToFile(rssItem.Argb8BitImage, imageSaveOptions.FolderPath, 
                rssItem.Id.AbsoluteUri, imageSaveOptions.ImageInfoSettings);
            return this.efEntityMapper.ToRssItemEfEntity(rssItem, imageDestinationPath.FullFilePath);
        }


        ////////////////////////////////////////////////////////////////////////


        private DestinationPath SaveImageToFile(byte[,,] image, string imageFolderPath, string imageFileName, ImageSettings<ImageInfo> imageInfoSettings)
        {
            var imageFilePath = new FilePath(imageFolderPath, imageFileName);
            DestinationPath imageDestinationPath = this.imageFileSaverCoordinator.Save8BitImageToFile(image, imageInfoSettings, imageFilePath);
            return imageDestinationPath;
        }

    }
}
