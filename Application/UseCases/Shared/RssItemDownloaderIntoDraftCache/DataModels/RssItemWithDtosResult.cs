using Application.OverallDataModels.OverallDto.CompositeDto;
using Application.UseCases.Shared.SharedDataModels.Class;

namespace Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels
{
    public class RssItemWithDtosResult : OperationResultBase
    {

        public IList<RssItemWithDto> CreatedRssItemWithDtos { get; init; }


        ////////////////////////////////////////////////////////////////


        public RssItemWithDtosResult() 
        {
        }


        public RssItemWithDtosResult(IList<RssItemWithDto> createdRssItemWithDtos, IList<Exception> creationErrors, bool wasUserCancelled) : 
            base(creationErrors, wasUserCancelled)
        {
            this.CreatedRssItemWithDtos = createdRssItemWithDtos;
        }

    }
}
