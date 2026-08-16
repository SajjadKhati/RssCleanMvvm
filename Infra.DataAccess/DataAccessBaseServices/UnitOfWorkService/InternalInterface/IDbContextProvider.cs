using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.InternalInterface
{
    internal interface IDbContextProvider
    {

        DbContext DbContext { get; init; }

    }
}
