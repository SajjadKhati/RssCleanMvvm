using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface
{
    public interface IRssItemDtoQuery : IDisposable
    {

        bool IsExist(string rssItemId);

        RssItemDto FindByKey(string rssItemId);

        IEnumerable<RssItemDto> FindByFeedId(string rssFeedId);

    }
}
