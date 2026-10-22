using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.DataAccessProject.RssItemDtoFactory.Interface
{
    public interface IRssItemDtoFactory
    {
        RssItemDto CreateRssItemDto(RssItem rssItem, ImageSaveOptions imageSaveOptions, bool isUnsaved);
    }
}
