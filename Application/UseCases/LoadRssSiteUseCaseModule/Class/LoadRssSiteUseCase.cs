using Application.OverallDataModels.OverallDto.EntityDto;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.Interface;
using Application.UseCases.LoadRssSiteUseCaseModule.Interface;

namespace Application.UseCases.LoadRssSiteUseCaseModule.Class
{
    public class LoadRssSiteUseCase : ILoadRssSiteUseCase
    {

        private IRssSiteQueryFactory rssSiteQueryFactory;


        /////////////////////////////////////////////////////////////////////////////


        public LoadRssSiteUseCase(IRssSiteQueryFactory rssSiteQueryFactory)
        {
            this.rssSiteQueryFactory = rssSiteQueryFactory;
        }


        /////////////////////////////////////////////////////////////////////////////


        public async Task<IList<RssSiteDto>> ExecuteAsync()
        {
            return await Task.Run(() => this.Execute());
        }


        private IList<RssSiteDto> Execute()
        {
            using IRssSiteQuery rssSiteQuery = this.rssSiteQueryFactory.Create();
            return rssSiteQuery.FindAllEagerLoading();
        }

    }
}
