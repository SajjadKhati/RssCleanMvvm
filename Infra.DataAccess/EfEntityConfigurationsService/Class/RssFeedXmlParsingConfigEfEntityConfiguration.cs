using Infra.DataAccess.EfEntities.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.DataAccess.EfEntityConfigurationsService.Class
{
    /// در این کلاس ها که RssFeedXmlParsingConfigEfEntityConfiguration هستند ، مقدار پیش فرض برای این جدول ها در دیتابیس در نظر گرفتیم .
    public class RssFeedXmlParsingConfigEfEntityConfiguration : IEntityTypeConfiguration<RssFeedXmlParsingConfigEfEntity>
    {

        public void Configure(EntityTypeBuilder<RssFeedXmlParsingConfigEfEntity> builder)
        {
            RssFeedXmlParsingConfigEfEntity[] rssFeedXmlParsingConfigEfEntity = this.GetDefaultRssFeedXmlParsingConfigEfEntities();
            builder.HasData(rssFeedXmlParsingConfigEfEntity);
        }


        private RssFeedXmlParsingConfigEfEntity[] GetDefaultRssFeedXmlParsingConfigEfEntities()
        {
            return new RssFeedXmlParsingConfigEfEntity[]
            {
                #region خبرگزاری تسنیم
                // صفحه اصلی :
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/0/AllStories").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "guid",
                        "link",
                        "category",
                        "title",
                        "description",
                        "pubDate",
                        "{http://search.yahoo.com/mrss/}thumbnail"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                // آخرین خبرهای روز :
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "guid",
                        "link",
                        "category",
                        "title",
                        "description",
                        "pubDate",
                        "{http://search.yahoo.com/mrss/}thumbnail"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                // سیاسی :
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/1/0/0/0").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "guid",
                        "link",
                        "category",
                        "title",
                        "description",
                        "pubDate",
                        "{http://search.yahoo.com/mrss/}thumbnail"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                // پر بیننده ترین اخبار امام و رهبری :
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feed/1127/0/7/0/MostPupolar").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "guid",
                        "link",
                        "category",
                        "title",
                        "description",
                        "pubDate",
                        "{http://search.yahoo.com/mrss/}thumbnail"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                // بین الملل :
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.tasnimnews.ir/fa/rss/feeds/8/0/0/0").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "guid",
                        "link",
                        "category",
                        "title",
                        "description",
                        "pubDate",
                        "{http://search.yahoo.com/mrss/}thumbnail"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                // فرهنگی :
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId =  new Uri("https://www.tasnimnews.ir/fa/rss/feeds/4/0/0/0").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "guid",
                        "link",
                        "category",
                        "title",
                        "description",
                        "pubDate",
                        "{http://search.yahoo.com/mrss/}thumbnail"
                    },
                    AttributeNames = new List<string>{ "url" }
                },
                #endregion

                #region باشگاه خبرنگاران جوان
                /// کل اخبار
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/allnews").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// سیاسی
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/3").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// بین الملل
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/9").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// اقتصادی
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/6").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// فرهنگی هنری
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/4").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// علمی پزشکی
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/7").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// فیلم و صوت
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.yjc.ir/fa/rss/11").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },
                #endregion

                #region خبرگزاری صدا و سیما
                /// کل اخبار
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/allnews").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// سیاسی
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/5").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// بین الملل
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/41").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// اقتصادی
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/6").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// علمی فرهنگی 
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/8").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// ورزشی
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/4").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },

                /// فیلم
                new RssFeedXmlParsingConfigEfEntity
                {
                    RssFeedEfEntityId = new Uri("https://www.irib-news.ir/fa/rss/3").AbsoluteUri,
                    ParentElementName = "item",
                    ChildElementNames = new List<string>()
                    {
                        "title",
                        "link",
                        "description",
                        "enclosure",
                        "pubDate"
                    },
                    AttributeNames = new List<string>{ "url" }
                },
                #endregion
            };
        }

    }
}
