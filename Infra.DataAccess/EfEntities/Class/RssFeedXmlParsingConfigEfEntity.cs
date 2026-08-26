using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.DataAccess.EfEntities.Class
{
    [Table("RssFeedXmlParsingConfigTable")]
    public class RssFeedXmlParsingConfigEfEntity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key()]
        [MaxLength(2048)]
        public string RssFeedEfEntityId { get; set; }

        public virtual RssFeedEfEntity RssFeedEfEntity { get; set; }


        [MaxLength(1024)]
        [Required()]
        public string ParentElementName { get; set; }

        public IList<string> ChildElementNames { get; set; }

        public IList<string>? AttributeNames { get; set; } 

    }
}
