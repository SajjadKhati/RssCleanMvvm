using Infra.DataAccess.EfEntities.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.DataAccess.RepositoryServices.Class.ProgrameRepositories.Configurations
{
    /// در این کلاس ها که RssFeedEfEntityConfiguration و RssSiteEfEntityConfiguration هستند ، مقدار پیش فرض برای این جدول ها در دیتابیس در نظر گرفتیم .
    public class RssSiteEfEntityConfiguration : IEntityTypeConfiguration<RssSiteEfEntity>
    {

        public void Configure(EntityTypeBuilder<RssSiteEfEntity> builder)
        {
            RssSiteEfEntity[] rssSiteEfEntities = this.GetDefaultRssSiteEfEntities();
            builder.HasData(rssSiteEfEntities);
        }


        private RssSiteEfEntity[] GetDefaultRssSiteEfEntities()
        {
            return new RssSiteEfEntity[]
            {
                new RssSiteEfEntity
                {
                    Id = new Uri("http://www.tasnimnews.ir/").AbsoluteUri,
                    SiteName = "خبرگزاری تسنیم"
                }
            };
        }

    }
}
