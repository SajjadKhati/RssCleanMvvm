using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.DataAccess.EfEntities.Class
{
    [Table("RssItemTable")]
    [Index(nameof(RssFeedEfEntityId))]
    [Index(nameof(PublishDate))]
    public class RssItemEfEntity
    {
        /// پارامتر ورودی اتریبیوت DatabaseGenerated ، باعث میشه که به EF بگیم که آیا خود دیتابیس بصورت اتوماتیک برای این فیلد ، مقدار مشخص کنه یا نه . که مخصوصا به درد فیلد شناسه میخوره .
        /// هر چند ، مقدار نوع string را EF بصورت اتوماتیک مقداری برایش تولید نمیکنه و صریحا باید خودمون مقدار بدیم اما برای شفاف سازی بیشتر ، اتریبوتِ DatabaseGenerated آورده شد .
        /// همچنین اتریبیوتِ Key هم نیاز نیست چون با نام پروپرتی ، اون را به عنوان کلید اصلی در نظر میگیره .
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key()]
        [MaxLength(2048)]
        public string Id { get; set; }

        [Required()]
        [MaxLength(2048)]
        public string RssFeedEfEntityId { get; set; }

        public virtual RssFeedEfEntity RssFeedEfEntity { get; set; }

        [Required()]
        [MaxLength(2048)]
        public string Link { get; set; }

        [MaxLength(512)]
        public string? Category { get; set; }

        [MaxLength(2048)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime PublishDate { get; set; }

        [Required()]
        [MaxLength(512)]
        public string ImageFullFilePath { get; set; }

    }
}
