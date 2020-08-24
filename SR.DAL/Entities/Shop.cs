using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Shop")]
    public class Shop : Entity
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public long BusinessId { get; set; }

        public Business Business { get; set; }
    }
}
