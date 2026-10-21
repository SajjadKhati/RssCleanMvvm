using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Shared.SharedDataModels.Class;

namespace Application.UseCases.Entry.DownLoadRssItems.DataModels
{
    public class RssItemDtosResult : OperationResultBase
    {

        public IList<RssItemDto> CreatedRssItemDtos { get; init; }


        ////////////////////////////////////////////////////////////////


        public RssItemDtosResult(IList<RssItemDto> createdRssItemDtos)
        {
            this.CreatedRssItemDtos = createdRssItemDtos;
        }


        public RssItemDtosResult(IList<RssItemDto> createdRssItemDtos, IList<Exception> creationErrors, bool wasUserCancelled) : 
            base(creationErrors, wasUserCancelled)
        {
            this.CreatedRssItemDtos = createdRssItemDtos;
        }

    }
}
