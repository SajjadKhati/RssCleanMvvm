using Infra.DataAccess.EfEntities.Class;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.DbContextService.Class
{
    public class RssDbContext : DbContext
    {

        public virtual DbSet<RssSiteEfEntity> RssSiteDbSet { get; set; }

        public virtual DbSet<RssItemEfEntity> RssItemDbSet { get; set; }


        private string connectionString;

        private bool shouldUseLazyLoading;


        public RssDbContext(string connectionString, bool shouldUseLazyLoading = false)
        {
            this.connectionString = connectionString;
            this.shouldUseLazyLoading = shouldUseLazyLoading;
        }


        //public RssDbContext()
        //{
        //    this.connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=RssDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        //    //      this.connectionString = @"Server=.;Database=RssDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        //    //    this.connectionString = @"Server=.;Database=RssDb;Trusted_Connection=True;Encrypt=False;";
        //    this.shouldUseLazyLoading = false;
        //}


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(this.connectionString);
            optionsBuilder.UseLazyLoadingProxies(this.shouldUseLazyLoading);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /// فقط خط زیر ، یعنی اسمبلی ای را بگرد که کلاس RssDbContext درونش هست (یعنی همین اسمبلی و پروژه ی Infra.DataAccess را میگرده) . -
            /// و سپس ، کلاس هایی را که رابط IEntityTypeConfiguration را پیاده سازی کردند را ازش بصورت Reflection شی بساز و متد Configure در اینها را اجرا کن .
            /// در این کلاس ها که RssFeedEfEntityConfiguration و RssSiteEfEntityConfiguration هستند ، مقدار پیش فرض برای این جدول ها در دیتابیس در نظر گرفتیم .
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RssDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
