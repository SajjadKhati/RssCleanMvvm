using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infra.DataAccess.DataAccessBaseServices.RepositoryQueryBaseService.Class
{
    /// <summary>
    /// ⚠️ قاعده: اشیاء این کلاس (و مشتق‌هایش) نباید بین نخ‌ها به اشتراک گذاشته شوند.
    /// 
    /// DbContext یک "شیء جلسه‌ای" است — یعنی مقدار فیلدها یا همان حالت داخلی آن (Change Tracker، 
    /// Transaction، Connection) ممکن است که در طول چند متد تغییر ‌کند و این حالت بین متدها به اشتراک گذاشته ‌شود.
    /// 
    /// بنابراین:
    /// - thread-safe نیست.
    /// - هرگز در فیلد کلاس ذخیره نشود.
    /// - هر متد باید DbContext خودش را با using بسازد.
    /// - قفل (قفل بین نخ ها) هم راه‌حل نیست — چون invariant آن جلسه‌ای است، نه متدی. 
    /// 
    /// مثال نقض: اگر entity توسط نخ A به Change Tracker اضافه شود و نخ B 
    /// SaveChanges بزند، entity نخ A هم ذخیره می‌شود (داده خراب).
    /// </summary>
    public class DbContextBase
    {

        /// <summary>
        /// ⚠️ این DbContext، ممکن است که در طول عمر کل شیء (چند متد) استفاده ‌شود.
        /// پس این شیء نباید بین نخ‌ها به اشتراک گذاشته شود.
        /// برای جزئیات، توضیحات کلاس را ببینید.
        /// </summary>
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
