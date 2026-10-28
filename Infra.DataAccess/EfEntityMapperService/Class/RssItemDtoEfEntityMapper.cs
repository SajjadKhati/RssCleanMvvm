using Application.InfrastructureContracts.SharedProject.DateTimes.PersianCalenderConvertor.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.EfEntityMapperService.Class
{
    public class RssItemDtoEfEntityMapper
    {

        private IPersianCalenderConvertor persianCalenderConvertor;


        ///////////////////////////////////////////////////////////////////


        public RssItemDtoEfEntityMapper(IPersianCalenderConvertor persianCalenderConvertor)
        {
            this.persianCalenderConvertor = persianCalenderConvertor;
        }


        ///////////////////////////////////////////////////////////////////


        public RssItemDto ToRssItemDto(RssItemEfEntity rssItemEfEntity, bool isUnsaved)
        {
            return new RssItemDto
            {
                Id = rssItemEfEntity.Id,
                Link = rssItemEfEntity.Link,
                Category = rssItemEfEntity.Category,
                Title = rssItemEfEntity.Title,
                Description = rssItemEfEntity.Description,
                PublishDate = rssItemEfEntity.PublishDate,
                PersianPublishDate = this.persianCalenderConvertor.ToPersianDateString(rssItemEfEntity.PublishDate),
                ImageFullFilePath = rssItemEfEntity.ImageFullFilePath,
                IsUnsaved = isUnsaved
            };
        }

    }
}
