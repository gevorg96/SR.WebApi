using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("OrderDetail")]
    public class OrderDetail : Entity
    {
        [Required]
        public long OrderId { get; set; }

        [Required]
        public long ProdId { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public decimal Count { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
