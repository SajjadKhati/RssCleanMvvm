using Domain.DomainEntities.RssEntity.Class;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.EfEntityMapperService.Class
{
    public class RssItemEfEntityMapper
    {

        public RssItemEfEntity ToRssItemEfEntity(RssItem rssItem, string imageFullFilePath)
        {
            return new RssItemEfEntity
            {
                Id = rssItem.Id.AbsoluteUri,
                RssFeedEfEntityId = rssItem.RssFeedId.AbsoluteUri,
                Link = rssItem.Link.AbsoluteUri,
                Category = rssItem.Category,
                Title = rssItem.Title,
                Description = rssItem.Description,
                PublishDate = rssItem.PublishDate,
                ImageFullFilePath = imageFullFilePath
            };
        }


        public RssItem ToRssItem(RssItemEfEntity rssItemEfEntity, byte[,,] argb8BitImage)
        {
            return new RssItem(
                new Uri(rssItemEfEntity.Id),
                new Uri(rssItemEfEntity.RssFeedEfEntityId),
                new Uri(rssItemEfEntity.Link),
                argb8BitImage,
                rssItemEfEntity.Category,
                rssItemEfEntity.Title,
                rssItemEfEntity.Description,
                rssItemEfEntity.PublishDate);
        }


        public void UpdateRssItemEfEntity(RssItemEfEntity source, RssItemEfEntity target)
        {
            target.Id = source.Id;
            target.RssFeedEfEntityId = source.RssFeedEfEntityId;
            target.RssFeedEfEntity = source.RssFeedEfEntity;
            target.Link = source.Link;
            target.Category = source.Category;
            target.Title = source.Title;
            target.Description = source.Description;
            target.PublishDate = source.PublishDate;
            target.ImageFullFilePath = source.ImageFullFilePath;
        }

    }
}
