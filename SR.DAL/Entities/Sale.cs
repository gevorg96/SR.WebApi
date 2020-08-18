using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Sale")]
    public class Sale : Entity
    {
        [Required]
        public long ProdId { get; set; }

        [Required]
        public long BillId { get; set; }

        [Required]
        public decimal SaleSum { get; set; }

        [Required]
        public decimal SalesCount { get; set; }
        public long? UnitId { get; set; }

        [Required]
        public decimal SaleCost { get; set; }

        [Required]
        public decimal SaleProfit { get; set; }

        [Required]
        public decimal SalePrice { get; set; }

        public Bill Bill { get; set; }
        public Product Product { get; set; }
        public Unit Unit { get; set; }
    }
}
