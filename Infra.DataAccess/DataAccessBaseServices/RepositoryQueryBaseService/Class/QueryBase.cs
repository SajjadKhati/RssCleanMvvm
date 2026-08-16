using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class
{
    /// <summary>
    /// چون کلاس های Repository هم از از کلاس QueryBase ارث بریمیکنند و چون اغلب در Repository ، الگوی UnitOfWork که متد Save داره ، جداست و -
    /// بخاطر همین ، باید شی های اشتراکیِ DbContext ، هم در سرویس Repository و هم در سرویس UnitOfWork استفاده بشود و
    /// از طرفی فقط یکی از این سرویس ها باید مسئول Dispose باشه و از طرفی هم چون ذخیره ی نهایی ، با سرویس UnitOfWork هست ؛ بنابراین سرویس UnitOfWork باید مسئول Dispose کردن باشه ، -
    /// و پس Repository نباید مسئول Dispose کردن باشه .
    /// 
    /// بنابراین نباید متد Dispose را در کلاس QueryBase تعریف کرد . چون وگرنه کلاس های Repository هم ازش ارث بری و ممکن هست اشتباها استفاده کنند .
    /// بنابراین کلاس DbContextOwningQueryBase ، رابط IDisposable را بخاطر همین پیاده سازی میکنه و همه یی کلاس های کوئریِ فرزند ، از کلاس DbContextOwningQueryBase ارث بری باید کنند .
    /// 
    /// در شرایط خاص مثل UnitOfWork و Query ، ما میتونیم مالکیتِ Disposable کردن را به این سرویس ها تغییر بدیم .
    /// یعنی سرویس Factory شون ، فقط شی DbContext را بسازه اما این سرویس ها ، مسئول Dispose کردنش باشند .
    /// </summary>
    public abstract class QueryBase
    {

        private DbContext dbContext;


        ///////////////////////////////////////////////////////////////


        public QueryBase(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        ///////////////////////////////////////////////////////////////


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

    }
}
