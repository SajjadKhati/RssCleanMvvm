using Infra.DataAccess.EfEntities.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.DataAccess.EfEntityConfigurationsService.Class
{
    /// در این کلاس ها که RssFeedEfEntityConfiguration و RssSiteEfEntityConfiguration هستند ، مقدار پیش فرض برای این جدول ها در دیتابیس در نظر گرفتیم .
    public class RssFeedEfEntityConfiguration : IEntityTypeConfiguration<RssFeedEfEntity>
    {

        public void Configure(EntityTypeBuilder<RssFeedEfEntity> builder)
        {
            RssFeedEfEntity[] rssFeedEfEntities = this.GetDefaultRssFeedEfEntities();
            builder.HasData(rssFeedEfEntities);
        }


        private RssFeedEfEntity[] GetDefaultRssFeedEfEntities()
        {
            return new RssFeedEfEntity[]
            {
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/0/AllStories").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("http://www.tasnimnews.ir/").AbsoluteUri,
                    Title = "صفحه اصلی"
                },

                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("http://www.tasnimnews.ir/").AbsoluteUri,
                    Title = "آخرین خبرهای روز"
                },

                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/1/0/0/0").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("http://www.tasnimnews.ir/").AbsoluteUri,
                    Title = "سیاسی"
                },

                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.tasnimnews.ir/fa/rss/feed/1127/0/7/0/MostPupolar").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("http://www.tasnimnews.ir/").AbsoluteUri,
                    Title = "پر بیننده ترین اخبار امام و رهبری"
                },

                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/8/0/0/0").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("http://www.tasnimnews.ir/").AbsoluteUri,
                    Title = "بین الملل"
                },

                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/4/0/0/0").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("http://www.tasnimnews.ir/").AbsoluteUri,
                    Title = "فرهنگی"
                }
            };
        }


    }
}
