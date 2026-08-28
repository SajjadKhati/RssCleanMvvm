using Microsoft.EntityFrameworkCore;

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
