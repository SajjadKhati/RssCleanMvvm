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


        ///////////////////////////////////////////////////////////
        
        private ICollection<RssFeedEfEntity> ToRssFeedEfEntities(IList<RssFeed> rssFeeds)
        {
            return rssFeeds?.Select((RssFeed rssFeed) => this.rssFeedEfEntityMapper.ToRssFeedEfEntity(rssFeed))?.ToList();
        }


        private IList<RssFeed> ToRssFeeds(ICollection<RssFeedEfEntity> rssFeedEfEntities)
        {
            return rssFeedEfEntities?.Select((RssFeedEfEntity rssFeedEfEntity) => this.rssFeedEfEntityMapper.ToRssFeed(rssFeedEfEntity))?.ToList();
        }

    }
}
