using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.RssItemDtoEfEntityFactoryService.Interface
{
    public interface IRssItemEfEntityFactory
    {
        RssItemEfEntity CreateRssItemEfEntity(RssItem rssItem, ImageSaveOptions imageSaveOptions);
    }
}
