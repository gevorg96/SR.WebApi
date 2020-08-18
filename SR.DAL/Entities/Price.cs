using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Price")]
    public class Price : Entity
    {
        [Required]
        public long ProdId { get; set; }
        public long[] ShopId { get; set; }
        public decimal? PriceValue { get; set; }

        public Product Product { get; set; }
    }
}
