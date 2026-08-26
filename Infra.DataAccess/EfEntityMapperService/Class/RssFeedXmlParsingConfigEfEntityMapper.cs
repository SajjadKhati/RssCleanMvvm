using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.EfEntityMapperService.Class
{
    public class RssFeedXmlParsingConfigEfEntityMapper
    {

        public RssFeedXmlParsingConfig ToRssFeedXmlParsingConfig(RssFeedXmlParsingConfigEfEntity configEfEntity)
        {
            return new RssFeedXmlParsingConfig(new Uri(configEfEntity.RssFeedEfEntityId), configEfEntity.ParentElementName, 
                configEfEntity.ChildElementNames, configEfEntity.AttributeNames);
        }

    }
}
