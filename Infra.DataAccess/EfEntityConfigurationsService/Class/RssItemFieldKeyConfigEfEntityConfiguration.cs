using Infra.DataAccess.EfEntities.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.DataAccess.EfEntityConfigurationsService.Class
{
    /// در این کلاس ها که RssItemFieldKeyConfigEfEntityConfiguration هستند ، مقدار پیش فرض برای این جدول ها در دیتابیس در نظر گرفتیم .
    public class RssItemFieldKeyConfigEfEntityConfiguration : IEntityTypeConfiguration<RssItemFieldKeyConfigEfEntity>
    {

        public void Configure(EntityTypeBuilder<RssItemFieldKeyConfigEfEntity> builder)
        {
            RssItemFieldKeyConfigEfEntity[] rssItemFieldKeyConfigEfEntities = this.GetDefaultRssFeedXmlParsingConfigEfEntities();
            builder.HasData(rssItemFieldKeyConfigEfEntities);
        }


        private RssItemFieldKeyConfigEfEntity[] GetDefaultRssFeedXmlParsingConfigEfEntities()
        {
            return new RssItemFieldKeyConfigEfEntity[]
            {
                #region خبرگزاری تسنیم
                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/0/AllStories").AbsoluteUri,
                    IdKey = "guid",
                    LinkKey = "link",
                    CategoryKey = "category",
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories").AbsoluteUri,
                    IdKey = "guid",
                    LinkKey = "link",
                    CategoryKey = "category",
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/1/0/0/0").AbsoluteUri,
                    IdKey = "guid",
                    LinkKey = "link",
                    CategoryKey = "category",
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feed/1127/0/7/0/MostPupolar").AbsoluteUri,
                    IdKey = "guid",
                    LinkKey = "link",
                    CategoryKey = "category",
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/8/0/0/0").AbsoluteUri,
                    IdKey = "guid",
                    LinkKey = "link",
                    CategoryKey = "category",
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/4/0/0/0").AbsoluteUri,
                    IdKey = "guid",
                    LinkKey = "link",
                    CategoryKey = "category",
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },
                #endregion

                #region باشگاه خبرنگاران جوان
                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/allnews").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/3").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/9").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/6").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/4").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/7").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/11").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },
                #endregion

                #region خبرگزاری صدا و سیما
                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/allnews").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/5").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/41").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/6").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/8").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/4").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },

                new RssItemFieldKeyConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/3").AbsoluteUri,
                    IdKey = "link",
                    LinkKey = "link",
                    CategoryKey = null,
                    TitleKey = "title",
                    DescriptionKey = "description",
                    PublishDateKey = "pubDate",
                    Argb8BitImageKey = "url"
                },
                #endregion
            };
        }

    }
}
