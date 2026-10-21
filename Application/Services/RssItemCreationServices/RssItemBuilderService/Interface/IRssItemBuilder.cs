using Application.InfrastructureContracts.XmlProject.SharedDataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.Services.RssItemCreationServices.RssItemBuilderService.Interface
{
    public interface IRssItemBuilder
    {
        Task<RssItem> CreateRssItemAsync(IDictionary<string, string> rssItemDictionary, 
            RssItemWithFeedIdFieldKeyConfig rssItemWithFeedIdFieldKeyConfig, CancellationToken cancellationToken = default);
    }
}
