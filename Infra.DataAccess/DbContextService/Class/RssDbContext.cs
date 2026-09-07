using Application.InfrastructureContracts.SharedProject.Disposed.Interface;
using Infra.DataAccess.EfEntities.Class;
using Microsoft.EntityFrameworkCore;

namespace Infra.DataAccess.DbContextService.Class
{
    public class RssDbContext : DbContext, IDisposed
    {

        public virtual DbSet<RssSiteEfEntity> RssSiteDbSet { get; set; }

        public virtual DbSet<RssItemEfEntity> RssItemDbSet { get; set; }


        ///////////////////////////////////////////////////////////////////////////////////


        public event EventHandler Disposed;


        ///////////////////////////////////////////////////////////////////////////////////


        private string connectionString;

        private bool shouldUseLazyLoading;


        ///////////////////////////////////////////////////////////////////////////////////
        

        public RssDbContext(string connectionString, bool shouldUseLazyLoading = false)
        {
            this.connectionString = connectionString;
            this.shouldUseLazyLoading = shouldUseLazyLoading;
        }


        //public RssDbContext()
        //{
        //    this.connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=RssDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        //    this.shouldUseLazyLoading = false;
        //}


        ///////////////////////////////////////////////////////////////////////////////////


        public override void Dispose()
        {
            base.Dispose();
            this.OnDisposed(this, EventArgs.Empty);
        }


        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            this.OnDisposed(this, EventArgs.Empty);
        }


        ///////////////////////////////////////////////////////////////////////////////////


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


        protected void OnDisposed(object sender, EventArgs e) 
        {
            this.Disposed?.Invoke(sender, e);
        }

    }
}
