using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.Entry.DownLoadRssItems.DataModels
{
    public class RssItemDtosResult
    {

        public IList<RssItemDto> CreatedRssItemDtos { get; init; }

        public IList<Exception> CreationErrors { get; init; }


        ////////////////////////////////////////////////////////////////


        public RssItemDtosResult(IList<RssItemDto> createdRssItemDtos)
        {
            this.CreatedRssItemDtos = createdRssItemDtos;
        }


        public RssItemDtosResult(IList<RssItemDto> createdRssItemDtos, IList<Exception> creationErrors) : this(createdRssItemDtos)
        {
            this.CreationErrors = creationErrors;
        }

    }
}
