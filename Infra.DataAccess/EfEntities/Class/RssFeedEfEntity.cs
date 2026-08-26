using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.DataAccess.EfEntities.Class
{
    [Table("RssFeedTable")]
    public class RssFeedEfEntity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key()]
        [MaxLength(2048)]
        public string Id {  get; set; }

        [Required()]
        [MaxLength(2048)]
        public string RssSiteEfEntityId {  get; set; }

        public virtual RssSiteEfEntity RssSiteEfEntity { get; set; }

        /// <summary>
        /// این navigation collection property ، بیشتر برای صرفا ارتباطات در دیتابیس هست تا Ef ، ارتباطات دیتابیس را درست بشناسه .
        /// وگرنه چون در موجودیت RssFeed در دامنه وجود نداره ، نیاز به پُرر کردن نداره و همچنین نیاز به استفاده و پُر کردنش در Mapper اش هم نیست .
        /// </summary>
        public virtual ICollection<RssItemEfEntity> RssItemEfEntities {  get; set; }

        public virtual RssFeedXmlParsingConfigEfEntity RssFeedXmlParsingConfigEfEntity { get; set; }

        [Required()]
        [MaxLength(256)]
        public string Title { get; set; }

    }
}
