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

        private SaveChangesErrorInterpreter saveChangesErrorInterpreter;

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
            /// چون  کلاس SaveChangesErrorInterpreter ، فقط توسط UnitOfWork استفاده میشه ، اون را internal گرفتیم -
            /// و بنابراین در CompositionRoot در دسترس نیست که بشه تزریق کرد .
            this.saveChangesErrorInterpreter = new SaveChangesErrorInterpreter();
            this.isAlreadyDisposed = false;
        }


        //////////////////////////////////////////


        public int SaveChanges()
        {
            try
            {
                return this.DbContext.SaveChanges();
            }
            catch (DbUpdateException ex) when (this.saveChangesErrorInterpreter.IsIndexKeyTooLong(ex) == true)
            {
                IList<string> tooLongIds = this.saveChangesErrorInterpreter.ExtractTooLongRssItemIds(ex);

                /// پیام خطا شامل:
                ///   - علت (طول شناسه بیش از حد مجاز)
                ///   - حد مجاز (۴۵۰ کاراکتر)
                ///   - تعداد آیتم‌های مشکل‌دار
                ///   - لیست کامل شناسه‌های مشکل‌دار همراه با طول‌شان
                string idsList = string.Join(Environment.NewLine, tooLongIds.Select(id => $"  • {id}  ({id.Length} کاراکتر)"));

                throw new InvalidOperationException(
                    $"ذخیره‌سازی در پایگاه داده انجام نشد. " +
                    $"علت: طول شناسه (Id) برخی از آیتم‌های خبری از حد مجاز پایگاه داده " +
                    $"(۴۵۰ کاراکتر، به دلیل محدودیت ۹۰۰ بایتی clustered index در SQL Server) بیشتر است." +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"تعداد آیتم‌های مشکل‌دار: {tooLongIds.Count}" +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"شناسه‌های مشکل‌دار:" +
                    $"{Environment.NewLine}{idsList}\n\n",
                    ex);
            }
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
