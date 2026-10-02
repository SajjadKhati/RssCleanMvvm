using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface;
using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Interface;
using Application.OverallDataModels.OverallDto.EntityDto;
using Application.UseCases.Entry.LoadRssSite.Interface;

namespace Application.UseCases.Entry.LoadRssSite.Class
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
