using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.Entry.LoadRssSite.Interface
{
    public interface ILoadRssSiteUseCase
    {

        Task<IList<RssSiteDto>> ExecuteAsync();

    }
}
