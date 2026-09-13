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
                #region خبرگزاری تسنیم
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
                } ,
                #endregion

                #region باشگاه خبرنگاران جوان
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.yjc.ir/fa/rss/allnews").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.yjc.ir/").AbsoluteUri,
                    Title = "کل اخبار"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.yjc.ir/fa/rss/3").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.yjc.ir/").AbsoluteUri,
                    Title = "سیاسی"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.yjc.ir/fa/rss/9").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.yjc.ir/").AbsoluteUri,
                    Title = "بین الملل"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.yjc.ir/fa/rss/6").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.yjc.ir/").AbsoluteUri,
                    Title = "اقتصادی"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.yjc.ir/fa/rss/4").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.yjc.ir/").AbsoluteUri,
                    Title = "فرهنگی هنری"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.yjc.ir/fa/rss/7").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.yjc.ir/").AbsoluteUri,
                    Title = "علمی پزشکی"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.yjc.ir/fa/rss/11").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.yjc.ir/").AbsoluteUri,
                    Title = "فیلم و صوت"
                } ,
                #endregion

                #region خبرگزاری صدا و سیما
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.irib-news.ir/fa/rss/allnews").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.irib-news.ir/").AbsoluteUri,
                    Title = "کل اخبار"
                },

                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.irib-news.ir/fa/rss/5").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.irib-news.ir/").AbsoluteUri,
                    Title = "سیاسی"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.irib-news.ir/fa/rss/41").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.irib-news.ir/").AbsoluteUri,
                    Title = "بین الملل"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.irib-news.ir/fa/rss/6").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.irib-news.ir/").AbsoluteUri,
                    Title = "اقتصادی"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.irib-news.ir/fa/rss/8").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.irib-news.ir/").AbsoluteUri,
                    Title = "علمی فرهنگی"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.irib-news.ir/fa/rss/4").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.irib-news.ir/").AbsoluteUri,
                    Title = "ورزشی"
                } ,
                new RssFeedEfEntity
                {
                    Id = new Uri("https://www.irib-news.ir/fa/rss/3").AbsoluteUri,
                    RssSiteEfEntityId = new Uri("https://www.irib-news.ir/").AbsoluteUri,
                    Title = "فیلم"
                } ,
                #endregion
            };
        }


    }
}
