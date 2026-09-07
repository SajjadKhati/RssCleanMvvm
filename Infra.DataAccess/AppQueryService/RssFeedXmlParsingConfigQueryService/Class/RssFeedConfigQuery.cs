using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssFeedXmlParsingConfigQuery.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using System.Collections.ObjectModel;

namespace Infra.DataAccess.AppQueryService.RssFeedXmlParsingConfigQueryService.Class
{
    public class RssFeedConfigQuery : DbContextOwningQueryBase, IRssFeedConfigQuery
    {

        private RssFeedConfigEfEntityMapper rssFeedConfigEfEntityMapper;
        

        ///////////////////////////////////////////////////////////////////////
        

        public RssFeedConfigQuery(RssDbContext dbContext, RssFeedConfigEfEntityMapper rssFeedConfigEfEntityMapper) : base(dbContext)
        {
            this.rssFeedConfigEfEntityMapper = rssFeedConfigEfEntityMapper;
        }


        ///////////////////////////////////////////////////////////////////////


        public IReadOnlyDictionary<Uri, RssFeedConfig> FindAll()
        {
            IDictionary<Uri, RssFeedConfig> rssFeedConfigDictionary =
                this.FindAllEntity<RssFeedXmlParsingConfigEfEntity>()?.
                ToList()?.
                Select<RssFeedXmlParsingConfigEfEntity, RssFeedConfig>(xmlParsingConfigEfEntity => this.GetRssFeedConfig(xmlParsingConfigEfEntity))?.
                ToDictionary<RssFeedConfig, Uri>(rssFeedConfig => rssFeedConfig.RssFeedXmlParsingConfig.RssFeedId);

            return new ReadOnlyDictionary<Uri, RssFeedConfig>(rssFeedConfigDictionary);
        }


        ///////////////////////////////////////////////////////////////////////


        private RssFeedConfig GetRssFeedConfig(RssFeedXmlParsingConfigEfEntity xmlParsingConfigEfEntity)
        {
            RssItemFieldKeyConfigEfEntity rssItemFieldKeyConfigEfEntity =
                this.FindRssItemFieldKeyConfigEfEntityByFeedId(xmlParsingConfigEfEntity.RssFeedEfEntityId);
            return this.rssFeedConfigEfEntityMapper.ToRssFeedConfig(xmlParsingConfigEfEntity, rssItemFieldKeyConfigEfEntity);
        }


        private RssItemFieldKeyConfigEfEntity FindRssItemFieldKeyConfigEfEntityByFeedId(string rssFeedId)
        {
            return this.FindByKeyEntity<string, RssItemFieldKeyConfigEfEntity>(rssFeedId);
        }

    }
}
