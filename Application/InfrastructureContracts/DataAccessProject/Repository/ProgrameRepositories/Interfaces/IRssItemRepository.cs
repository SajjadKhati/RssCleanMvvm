using Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.DataModels;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.Interfaces
{
    public interface IRssItemRepository
    {

        void Add(RssItem rssItem, ImageSaveOptions imageSaveOptions);

        void Delete(RssItem rssItem);

        void Update(RssItem rssItem, ImageSaveOptions imageSaveOptions);

        RssItem FindByKey(string rssItemKey);

        IEnumerable<RssItem> FindAll();

    }
}
