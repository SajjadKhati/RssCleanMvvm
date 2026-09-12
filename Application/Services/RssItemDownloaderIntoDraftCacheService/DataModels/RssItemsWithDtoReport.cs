using Application.OverallDataModels.OverallDto.EntityDto;
using Application.Services.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemDownloaderIntoDraftCacheService.DataModels
{
    public class RssItemsWithDtoReport :  RssItemsReport
    {

        public IList<RssItemDto> CreatedRssItemDtos { get; init; }


        ////////////////////////////////////////////////////////////////


        public RssItemsWithDtoReport(IList<RssItem> createdRssItems, IList<RssItemDto> createdRssItemDtos) : base(createdRssItems)
        {
            this.CreatedRssItemDtos = createdRssItemDtos;
        }


        public RssItemsWithDtoReport(IList<RssItem> createdRssItems, IList<RssItemDto> createdRssItemDtos, 
            IList<Exception> creationErrors) : base(createdRssItems, creationErrors)
        {
            this.CreatedRssItemDtos = createdRssItemDtos;
        }

    }
}
