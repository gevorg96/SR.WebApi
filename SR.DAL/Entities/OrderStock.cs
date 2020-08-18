using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("OrderStock")]
    public class OrderStock : Entity
    {
        public long? OrderId { get; set; }
        public long? ProdId { get; set; }
        public decimal? CurrStocks { get; set; }

        [Required]
        public int ShopId { get; set; }

        public OrderDetail OrderDetail { get; set; }
    }
}
