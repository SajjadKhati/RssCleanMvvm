using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.RssItemDtoQuery.Interface
{
    public interface IRssItemDtoQuery : IDisposable
    {

        RssItemDto FindByKey(string key);

        IEnumerable<RssItemDto> FindAll();

    }
}
