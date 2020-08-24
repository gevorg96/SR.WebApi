using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Folder")]
    public class Folder : Entity
    {
        [Required]
        public long BusinessId { get; set; }
        public long? ParentId { get; set; }

        [Required]
        public string Name { get; set; }

        public override bool Equals(object obj) => obj is Folder && this != null && (obj as Folder).Id == Id;
    }
}
