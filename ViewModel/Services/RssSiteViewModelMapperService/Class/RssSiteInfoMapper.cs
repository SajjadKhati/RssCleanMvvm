using Application.OverallDataModels.OverallDto.EntityDto;
using ViewModel.DataModels;

namespace ViewModel.Services.RssSiteViewModelMapperService.Class
{
    public class RssSiteInfoMapper
    {

        public RssSiteInfo ToRssSiteInfo(RssSiteDto rssSiteDto)
        {
            IList<RssFeedInfo> rssFeedInfos = rssSiteDto.RssFeedDtos?.Select(rssFeedDto => new RssFeedInfo(rssFeedDto.Id, rssFeedDto.Title))?.ToList();
            return new RssSiteInfo(rssSiteDto.Id, rssFeedInfos, rssSiteDto.SiteName);
        }

    }
}
