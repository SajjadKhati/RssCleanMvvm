using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class
{
    public abstract class RepositoryBase : QueryBase
    {

        private DbContext dbContext;


        ///////////////////////////////////////////////////////////////


        public RepositoryBase(DbContext dbContext) : base(dbContext)
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


        public TNonQueryEntity AddEntity<TNonQueryEntity>(TNonQueryEntity entity) where TNonQueryEntity : class
        {
            DbSet<TNonQueryEntity> entityDbSet = this.dbContext.Set<TNonQueryEntity>();
            return entityDbSet?.Add(entity)?.Entity;
        }


        public TNonQueryEntity DeleteEntity<TNonQueryEntity>(TNonQueryEntity deleteEntity) where TNonQueryEntity : class
        {
            DbSet<TNonQueryEntity> entityDbSet = this.dbContext.Set<TNonQueryEntity>();
            return entityDbSet?.Remove(deleteEntity)?.Entity;
        }

    }
}
