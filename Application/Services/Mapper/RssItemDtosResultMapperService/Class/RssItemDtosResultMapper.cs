using Application.UseCases.Entry.DownLoadRssItems.DataModels;
using Application.UseCases.Shared.RssItemDownloaderIntoDraftCache.DataModels;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.OverallDataModels.OverallDto.CompositeDto;

namespace Application.Services.Mapper.RssItemDtosResultMapperService.Class
{
    public class RssItemDtosResultMapper
    {

        public RssItemDtosResult ToRssItemDtosResult(RssItemWithDtosResult rssItemWithDtosResult)
        {
            IList<RssItemDto> rssItemDtos = rssItemWithDtosResult.CreatedRssItemWithDtos?.Select(
                (RssItemWithDto rssItemWithDto) => rssItemWithDto.RssItemDto).ToList();
            return new RssItemDtosResult(rssItemDtos, rssItemWithDtosResult.CreationErrors, rssItemWithDtosResult.WasUserCancelled);
        }

    }
}
