using Application.OverallDataModels.OverallDto.EntityDto;
using Application.Services.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemDownloaderIntoDraftCacheService.DataModels
{
    public class RssItemsWithDtoBuildingResult :  RssItemsBuildingResult
    {

        public IList<RssItemDto> CreatedRssItemDtos { get; init; }


        ////////////////////////////////////////////////////////////////


        public RssItemsWithDtoBuildingResult(IList<RssItem> createdRssItems, IList<RssItemDto> createdRssItemDtos) : base(createdRssItems)
        {
            this.CreatedRssItemDtos = createdRssItemDtos;
        }


        public RssItemsWithDtoBuildingResult(IList<RssItem> createdRssItems, IList<RssItemDto> createdRssItemDtos, 
            IList<Exception> creationErrors) : base(createdRssItems, creationErrors)
        {
            this.CreatedRssItemDtos = createdRssItemDtos;
        }

    }
}
