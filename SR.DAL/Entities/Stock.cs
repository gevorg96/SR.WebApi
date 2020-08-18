using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Stock")]
    public class Stock : Entity
    {
        public long? ShopId { get; set; }

        [Required]
        public long ProdId { get; set; }
        public decimal? StockCount { get; set; }

        public Product Product { get; set; }
        public Shop Shop { get; set; }
    }
}
