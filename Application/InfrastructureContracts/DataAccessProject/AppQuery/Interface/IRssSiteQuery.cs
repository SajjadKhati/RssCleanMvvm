using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.Interface
{
    public interface IRssSiteQuery : IDisposable
    {

        IList<RssSiteDto> FindAllEagerLoading();

    }
}
