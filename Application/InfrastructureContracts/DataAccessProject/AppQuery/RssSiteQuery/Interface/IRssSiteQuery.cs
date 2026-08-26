using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Interface
{
    public interface IRssSiteQuery : IDisposable
    {

        IList<RssSiteDto> FindAllEagerLoading();

    }
}
