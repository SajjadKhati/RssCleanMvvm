using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemCreationServices.RssItemFactoryService.Interface
{
    public interface IRssItemFactory
    {
        RssItem CreateRssItem(IDictionary<string, string> rssItemDictionary, Uri rssFeedId);
    }
}
