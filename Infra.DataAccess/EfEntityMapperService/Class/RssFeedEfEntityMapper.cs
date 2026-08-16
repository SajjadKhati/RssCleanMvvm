using Application.OverallDataModels.OverallDto.EntityDto;
using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.EfEntityMapperService.Class
{
    public class RssFeedEfEntityMapper
    {

        public RssFeedEfEntity ToRssFeedEfEntity(RssFeed rssFeed)
        {
            return new RssFeedEfEntity
            {
                Id = rssFeed.Id.AbsoluteUri,
                RssSiteEfEntityId = rssFeed.RssSiteId.AbsoluteUri,
                Title = rssFeed.Title
            };
        }


        public RssFeed ToRssFeed(RssFeedEfEntity rssFeedEfEntity)
        {
            return new RssFeed(
                new Uri(rssFeedEfEntity.Id),
                 new Uri(rssFeedEfEntity.RssSiteEfEntityId),
                rssFeedEfEntity.Title);
        }


        public RssFeedDto ToRssFeedDto(RssFeedEfEntity rssFeedEfEntity)
        {
            return new RssFeedDto
            {
                Id = rssFeedEfEntity.Id,
                Title = rssFeedEfEntity.Title
            };
        }

    }
}
