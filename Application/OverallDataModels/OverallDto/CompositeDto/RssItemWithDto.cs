using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.OverallDataModels.OverallDto.CompositeDto
{
    public class RssItemWithDto
    {

        public RssItem RssItem { get; set; }

        public RssItemDto RssItemDto { get; set; }


        /////////////////////////////////////////////////////////////////////////


        public RssItemWithDto()
        {

        }

        public RssItemWithDto(RssItem rssItem, RssItemDto rssItemDto)
        {
            this.RssItem = rssItem;
            this.RssItemDto = rssItemDto;
        }

    }
}
