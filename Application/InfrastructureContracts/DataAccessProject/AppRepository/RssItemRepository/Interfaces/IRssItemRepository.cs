using Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.DataModels;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.DataAccessProject.AppRepository.RssItemRepository.Interfaces
{
    public interface IRssItemRepository
    {

        void Add(RssItem rssItem, ImageSaveOptions imageSaveOptions);

        void Delete(RssItem rssItem);

        void Update(RssItem rssItem, ImageSaveOptions imageSaveOptions);

        RssItem FindByKey(string rssItemKey);

        IEnumerable<RssItem> FindByFeedId(string rssFeedId);

    }
}
