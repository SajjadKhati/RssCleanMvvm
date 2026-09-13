using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.DataAccess.EfEntities.Class
{
    [Table("RssItemFieldKeyConfigTable")]
    public class RssItemFieldKeyConfigEfEntity
    {

        /// <summary>
        /// این پروپرتی ، هم کلید اصلی و هم کلید خارجی هست .
        /// کلید خارجیِ جدول RssFeedEfEntity هست .
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key()]
        [MaxLength(2048)]
        public string RssFeedEfEntityId { get; set; }

        public virtual RssFeedEfEntity RssFeedEfEntity { get; set; }


        [MaxLength(256)]
        [Required()]
        public string IdKey { get; set; }

        [MaxLength(256)]
        [Required()]
        public string LinkKey { get; set; }

        [MaxLength(256)]
        public string? CategoryKey { get; set; }

        [MaxLength(256)]
        [Required()]
        public string TitleKey { get; set; }

        [MaxLength(256)]
        [Required()]
        public string DescriptionKey { get; set; }

        [MaxLength(256)]
        [Required()]
        public string PublishDateKey { get; set; }

        [MaxLength(256)]
        [Required()]
        public string Argb8BitImageKey { get; set; }

    }
}
