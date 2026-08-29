using Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.Mapper
{
    public class RssItemDtoMapper : IRssItemDtoMapper
    {

        public RssItemDto ToRssItemDto(RssItem rssItem, string imageFullFilePath)
        {
            return new RssItemDto
            {
                Id = rssItem.Id.AbsoluteUri,
                Link = rssItem.Link.AbsoluteUri,
                Category = rssItem.Category,
                Title = rssItem.Title,
                Description = rssItem.Description,
                PublishDate = rssItem.PublishDate,
                ImageFullFilePath = imageFullFilePath
            };
        }

    }
}
