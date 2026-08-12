using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.UnitOfWorkService.InternalInterface
{
    internal interface IDbContextProvider
    {

        DbContext DbContext { get; init; }

    }
}
