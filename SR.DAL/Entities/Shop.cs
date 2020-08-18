using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Shop")]
    public class Shop : Entity
    {
        [Required]
        public string ShopAddress { get; set; }

        [Required]
        public string ShopName { get; set; }

        [Required]
        public long BusinessId { get; set; }

        public Business Business { get; set; }
    }
}
