using Application.InfrastructureContracts.DataAccessProject.AppQuery.Interface;

namespace Application.InfrastructureContracts.DataAccessProject.AppQuery.Factory.Interface
{
    public interface IRssSiteQueryFactory
    {

        IRssSiteQuery Create(bool shouldUseLazyLoading = false);

    }
}
