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
                        "{http://search.yahoo.com/mrss/}content"
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
                        "{http://search.yahoo.com/mrss/}content"
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
                        "{http://search.yahoo.com/mrss/}content"
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
                        "{http://search.yahoo.com/mrss/}content"
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
                        "{http://search.yahoo.com/mrss/}content"
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
                        "{http://search.yahoo.com/mrss/}content"
                    },
                    AttributeNames = new List<string>{ "url" }
                },


            };
        }

    }
}
