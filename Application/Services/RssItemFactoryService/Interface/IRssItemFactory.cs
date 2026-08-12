using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemFactoryService.Interface
{
    public interface IRssItemFactory
    {

        RssItem CreateRssItem(IDictionary<string, string> rssItemrssItemDictionaryFields);

    }
}
