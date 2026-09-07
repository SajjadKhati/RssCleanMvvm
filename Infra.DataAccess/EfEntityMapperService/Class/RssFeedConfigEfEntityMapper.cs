using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Infra.DataAccess.EfEntities.Class;

namespace Infra.DataAccess.EfEntityMapperService.Class
{
    public class RssFeedConfigEfEntityMapper
    {

        public RssFeedConfig ToRssFeedConfig(RssFeedXmlParsingConfigEfEntity xmlParsingConfigEfEntity,
            RssItemFieldKeyConfigEfEntity fieldKeyConfigEfEntity)
        {
            return new RssFeedConfig
            {
                RssFeedXmlParsingConfig = this.ToRssFeedXmlParsingConfig(xmlParsingConfigEfEntity),
                RssItemWithFeedIdFieldKeyConfig = this.ToRssItemFieldKeyConfig(fieldKeyConfigEfEntity)
            };
        }


        //////////////////////////////////////////////////////////////////////////////////////


        private RssFeedXmlParsingConfig ToRssFeedXmlParsingConfig(RssFeedXmlParsingConfigEfEntity xmlParsingConfigEfEntity)
        {
            Uri rssFeedId = new Uri(xmlParsingConfigEfEntity.RssFeedEfEntityId);
            RssFeedXmlParsingConfig rssFeedXmlParsingConfig = new RssFeedXmlParsingConfig(rssFeedId, xmlParsingConfigEfEntity.ParentElementName,
                xmlParsingConfigEfEntity.ChildElementNames, xmlParsingConfigEfEntity.AttributeNames);
            return rssFeedXmlParsingConfig;
        }


        private RssItemWithFeedIdFieldKeyConfig ToRssItemFieldKeyConfig(RssItemFieldKeyConfigEfEntity fieldKeyConfigEfEntity)
        {
            return new RssItemWithFeedIdFieldKeyConfig
            {
                RssFeedId = new Uri(fieldKeyConfigEfEntity.RssFeedEfEntityId),
                RssItemFieldKeyConfig = new RssItemFieldKeyConfig
                {
                    IdKey = fieldKeyConfigEfEntity.IdKey,
                    LinkKey = fieldKeyConfigEfEntity.LinkKey,
                    CategoryKey = fieldKeyConfigEfEntity.CategoryKey,
                    TitleKey = fieldKeyConfigEfEntity.TitleKey,
                    DescriptionKey = fieldKeyConfigEfEntity.DescriptionKey,
                    PublishDateKey = fieldKeyConfigEfEntity.PublishDateKey,
                    Argb8BitImageKey = fieldKeyConfigEfEntity.Argb8BitImageKey
                }
            };
        }

    }
}
