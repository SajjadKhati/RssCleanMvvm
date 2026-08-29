using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface
{
    public interface IRssItemDtoMapper
    {
        RssItemDto ToRssItemDto(RssItem rssItem, string imageFullFilePath);
    }
}
