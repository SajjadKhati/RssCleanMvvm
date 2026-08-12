using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infra.DataAccess.RepositoryServices.Class.RepositoryBases
{
    public class RepositoryBase
    {

        private DbContext dbContext;


        ///////////////////////////////////////////////////////////////


        public RepositoryBase(DbContext dbContext)
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


        public IQueryable<TQueryEntity> FindAllEntity<TQueryEntity>() where TQueryEntity : class
        {
            DbSet<TQueryEntity> entityDbSet = this.dbContext.Set<TQueryEntity>();
            return entityDbSet;
        }


        public TQueryEntity FindByKeyEntity<TKey, TQueryEntity>(TKey key) where TQueryEntity : class
        {
            DbSet<TQueryEntity> entityDbSet = this.dbContext.Set<TQueryEntity>();
            return entityDbSet?.Find(key);
        }


        public IQueryable<TQueryEntity> FindByAttributeEntity<TQueryEntity>(
            Expression<Func<TQueryEntity, bool>> conditionsExpression) where TQueryEntity : class
        {
            DbSet<TQueryEntity> entityDbSet = this.dbContext.Set<TQueryEntity>();
            return entityDbSet?.Where(conditionsExpression)?.AsQueryable();
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
