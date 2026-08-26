using Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Interface;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.RssSiteQuery.Factory.Interface
{
    public interface IRssSiteQueryFactory
    {

        IRssSiteQuery Create(bool shouldUseLazyLoading = false);

    }
}
