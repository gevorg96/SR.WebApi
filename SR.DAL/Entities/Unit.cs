using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Unit")]
    public class Unit : Entity
    {
        [Required]
        public string Name { get; set; }
    }
}
