using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.DataAccess.EfEntities.Class
{
    [Table("RssSiteTable")]
    public class RssSiteEfEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key()]
        [MaxLength(2048)]
        public string Id { get; set; }

        public virtual ICollection<RssFeedEfEntity> RssFeedEfEntities { get; set; }

        [Required()]
        [MaxLength(50)]
        public string SiteName { get; set; }

    }
}
