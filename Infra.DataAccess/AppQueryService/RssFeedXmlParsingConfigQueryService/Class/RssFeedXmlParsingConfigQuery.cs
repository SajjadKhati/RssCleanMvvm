using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssFeedXmlParsingConfigQuery.Interface;
using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class;
using Infra.DataAccess.DbContextService.Class;
using Infra.DataAccess.EfEntities.Class;
using Infra.DataAccess.EfEntityMapperService.Class;
using System.Collections.ObjectModel;

namespace Infra.DataAccess.AppQueryService.RssFeedXmlParsingConfigQueryService.Class
{
    public class RssFeedXmlParsingConfigQuery : DbContextOwningQueryBase, IRssFeedXmlParsingConfigQuery
    {

        private RssFeedXmlParsingConfigEfEntityMapper configEfEntityMapper;
        

        ///////////////////////////////////////////////////////////////////////
        

        public RssFeedXmlParsingConfigQuery(RssDbContext dbContext, RssFeedXmlParsingConfigEfEntityMapper configEfEntityMapper) : base(dbContext)
        {
            this.configEfEntityMapper = configEfEntityMapper;
        }


        ///////////////////////////////////////////////////////////////////////


        public IReadOnlyDictionary<Uri, RssFeedXmlParsingConfig> FindAll()
        {
            IDictionary<Uri, RssFeedXmlParsingConfig> rssFeedXmlParsingConfigDictionary = 
                this.FindAllEntity<RssFeedXmlParsingConfigEfEntity>()?.
                ToList()?.
                Select(configEfEntity=> this.configEfEntityMapper.ToRssFeedXmlParsingConfig(configEfEntity))?.
                ToDictionary<RssFeedXmlParsingConfig, Uri>(config=> config.RssFeedLink);

            return new ReadOnlyDictionary<Uri, RssFeedXmlParsingConfig>(rssFeedXmlParsingConfigDictionary);
        }

    }
}
