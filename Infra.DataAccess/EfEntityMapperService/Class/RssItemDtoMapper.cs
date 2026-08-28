using Application.OverallDataModels.OverallDto.EntityDto;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.EfEntityMapperService.Class
{
    public class RssItemDtoMapper
    {


        public RssItemDto ToRssItemDto(RssItemEfEntity rssItemEfEntity)
        {
            return new RssItemDto
            {
                Id = rssItemEfEntity.Id,
                Link = rssItemEfEntity.Link,
                Category = rssItemEfEntity.Category,
                Title = rssItemEfEntity.Title,
                Description = rssItemEfEntity.Description,
                PublishDate = rssItemEfEntity.PublishDate,
                ImageFullFilePath = rssItemEfEntity.ImageFullFilePath
            };
        }


        public RssItemEfEntity ToRssItemEfEntity(RssItemDto rssItemDto)
        {
            return new RssItemEfEntity
            {
                Id = rssItemDto.Id,
                Link = rssItemDto.Link,
                Category = rssItemDto.Category,
                Title = rssItemDto.Title,
                Description = rssItemDto.Description,
                PublishDate = rssItemDto.PublishDate,
                ImageFullFilePath = rssItemDto.ImageFullFilePath
            };
        }

    }
}
