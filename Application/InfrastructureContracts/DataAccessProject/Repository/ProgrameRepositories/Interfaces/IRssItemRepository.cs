using Application.InfrastructureContracts.ImageProject.ImageFileSaver.DataModels.Class;
using Domain.DomainEntities.RssEntity.Class;

namespace Application.InfrastructureContracts.DataAccessProject.Repository.ProgrameRepositories.Interfaces
{
    public interface IRssItemRepository
    {

        void Add(RssItem rssItem, string imageFolderPath, ImageFileSettings imageFileSettings);

        void Delete(RssItem rssItem);

        void Update(RssItem rssItem, string imageFolderPath, ImageFileSettings imageFileSettings);

        RssItem FindByKey(string rssItemKey);

        IEnumerable<RssItem> FindAll();

    }
}
