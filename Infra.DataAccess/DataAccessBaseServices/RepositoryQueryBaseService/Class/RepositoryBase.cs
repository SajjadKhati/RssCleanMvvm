using Microsoft.EntityFrameworkCore;

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
    public abstract class RepositoryBase : QueryBase
    {

        /// <summary>
        /// ⚠️ این DbContext، ممکن است که در طول عمر کل شیء (چند متد) استفاده ‌شود.
        /// پس این شیء نباید بین نخ‌ها به اشتراک گذاشته شود.
        /// برای جزئیات، توضیحات کلاس را ببینید.
        /// </summary>
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
