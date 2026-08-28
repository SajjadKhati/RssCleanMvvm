using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class
{
    public class DbContextBase
    {

        private DbContext dbContext;


        ///////////////////////////////////////////////////////////////


        public DbContextBase(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        ///////////////////////////////////////////////////////////////


        public bool AttachEntityToDbContext<TEntity>(TEntity entity, EntityState entityState = EntityState.Unchanged)
            where TEntity : class
        {
            EntityEntry<TEntity> entityEntry = this.dbContext.Entry(entity);
            if (entityEntry.State == EntityState.Detached)
            {
                entityEntry = this.dbContext.Attach(entity);
                entityEntry.State = entityState;
                return true;
            }
            return false;
        }

    }
}
