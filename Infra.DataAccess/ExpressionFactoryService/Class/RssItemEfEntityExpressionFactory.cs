using Infra.DataAccess.EfEntities.Class;
using System.Linq.Expressions;

namespace Infra.DataAccess.ExpressionFactoryService.Class
{
    public class RssItemEfEntityExpressionFactory
    {

        public Expression<Func<RssItemEfEntity, bool>> ExpressionByFeedId(string rssFeedId)
        {
            return rssItemEfEntity => rssItemEfEntity.RssFeedEfEntityId == rssFeedId;
        }


        public Func<RssItemEfEntity, bool> FuncByFeedId(string rssFeedId)
        {
            return rssItemEfEntity => rssItemEfEntity.RssFeedEfEntityId == rssFeedId;
        }

    }
}
