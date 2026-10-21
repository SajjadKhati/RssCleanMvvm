using Application.InfrastructureContracts.SharedProject.DateTimes.PersianCalenderConvertor.Interface;
using Application.InfrastructureContracts.SharedProject.SharedMapper.RssItemDtoMapper.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.Mapper.RssItemDtoMapperService.Class
{
    public class RssItemDtoMapper : IRssItemDtoMapper
    {

        private IPersianCalenderConvertor persianCalenderConvertor;


        ///////////////////////////////////////////////////////////////////


        public RssItemDtoMapper(IPersianCalenderConvertor persianCalenderConvertor)
        {
            this.persianCalenderConvertor = persianCalenderConvertor;
        }


        ///////////////////////////////////////////////////////////////////


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
                PersianPublishDate = this.persianCalenderConvertor.ToPersianDateString(rssItem.PublishDate),
                ImageFullFilePath = imageFullFilePath
            };
        }

    }
}
