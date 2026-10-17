using Application.InfrastructureContracts.DataAccessProject.DataAccessBase.UnitOfWork.Interfaces;
using Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.InternalInterface;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.DataAccessBaseServices.UnitOfWorkService.Class
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
    public class UnitOfWork : IUnitOfWork, IDbContextProvider
    {

        /// <summary>
        /// ⚠️ این DbContext، ممکن است که در طول عمر کل شیء (چند متد) استفاده ‌شود.
        /// پس این شیء نباید بین نخ‌ها به اشتراک گذاشته شود.
        /// برای جزئیات، توضیحات کلاس را ببینید.
        /// </summary>
        private DbContext _dbContext;

        private bool isAlreadyDisposed;


        //////////////////////////////////////////


        public DbContext DbContext
        {
            get 
            {
                if(this.isAlreadyDisposed == true)
                    throw new ObjectDisposedException(nameof(this.DbContext), "این شی قبلا Dispose شده است .");

                return this._dbContext; 
            }
            init
            {
                this._dbContext = value;
            }
        }


        //////////////////////////////////////////


        public UnitOfWork(DbContext dbContext)
        {
            this.DbContext = dbContext;
            this.isAlreadyDisposed = false;
        }


        //////////////////////////////////////////


        public int SaveChanges()
        {
            return this.DbContext.SaveChanges();
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (this.isAlreadyDisposed == false)
            {
                if (disposing == true)
                {
                    this.DbContext?.Dispose();
                }

                this.isAlreadyDisposed = true;
            }
        }

    }
}
