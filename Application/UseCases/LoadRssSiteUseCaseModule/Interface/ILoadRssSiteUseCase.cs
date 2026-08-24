using Application.OverallDataModels.OverallDto.EntityDto;

namespace Application.UseCases.LoadRssSiteUseCaseModule.Interface
{
    public interface ILoadRssSiteUseCase
    {

        Task<IList<RssSiteDto>> ExecuteAsync();

    }
}
