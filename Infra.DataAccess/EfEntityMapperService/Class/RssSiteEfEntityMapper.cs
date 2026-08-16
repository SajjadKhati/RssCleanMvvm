using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.EfEntityMapperService.Class
{
    public class RssSiteEfEntityMapper
    {

        private RssFeedEfEntityMapper rssFeedEfEntityMapper;


        ///////////////////////////////////////////////////////////


        public RssSiteEfEntityMapper(RssFeedEfEntityMapper rssFeedEfEntityMapper)
        {
            this.rssFeedEfEntityMapper = rssFeedEfEntityMapper;
        }


        ///////////////////////////////////////////////////////////


        public RssSiteEfEntity ToRssSiteEfEntity(RssSite rssSite)
        {
            return new RssSiteEfEntity
            {
                Id = rssSite.Id.AbsoluteUri,
                RssFeedEfEntities = this.ToRssFeedEfEntities(rssSite.RssFeeds),
                SiteName = rssSite.SiteName,
            };
        }


        public RssSite ToRssSite(RssSiteEfEntity rssSiteEfEntity)
        {
            return new RssSite(
                new Uri(rssSiteEfEntity.Id),
                this.ToRssFeeds(rssSiteEfEntity.RssFeedEfEntities),
                rssSiteEfEntity.SiteName);
        }


        public RssSiteDto ToRssSiteDto(RssSiteEfEntity rssSiteEfEntity)
        {
            return new RssSiteDto
            {
                Id = rssSiteEfEntity.Id,
                RssFeedDtos = this.ToRssFeedDtos(rssSiteEfEntity.RssFeedEfEntities),
                SiteName = rssSiteEfEntity.SiteName
            };
        }


        ///////////////////////////////////////////////////////////

        private ICollection<RssFeedEfEntity> ToRssFeedEfEntities(IList<RssFeed> rssFeeds)
        {
            return rssFeeds?.Select(rssFeed => this.rssFeedEfEntityMapper.ToRssFeedEfEntity(rssFeed))?.ToList();
        }


        private IList<RssFeed> ToRssFeeds(ICollection<RssFeedEfEntity> rssFeedEfEntities)
        {
            return rssFeedEfEntities?.Select(rssFeedEfEntity => this.rssFeedEfEntityMapper.ToRssFeed(rssFeedEfEntity))?.ToList();
        }


        private IList<RssFeedDto> ToRssFeedDtos(ICollection<RssFeedEfEntity> rssFeedEfEntities)
        {
            return rssFeedEfEntities?.Select(rssFeedEfEntity => this.rssFeedEfEntityMapper.ToRssFeedDto(rssFeedEfEntity))?.ToList();
        }

    }
}
