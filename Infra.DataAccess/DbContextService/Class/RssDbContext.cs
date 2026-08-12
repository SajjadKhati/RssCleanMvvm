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
        //      this.connectionString = @"Server=.;Database=RssDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        ////    this.connectionString = @"Server=.;Database=RssDb;Trusted_Connection=True;Encrypt=False;";
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
            base.OnModelCreating(modelBuilder);
        }

    }
}
