using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("InvoiceStock")]
    public class InvoiceStock : Entity
    {
        public long? OrderId { get; set; }
        public long? ProdId { get; set; }
        public decimal? CurrStocks { get; set; }

        [Required]
        public int ShopId { get; set; }

        public InvoiceDetail InvoiceDetail { get; set; }
    }
}
