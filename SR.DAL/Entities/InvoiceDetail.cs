using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("InvoiceDetail")]
    public class InvoiceDetail : Entity
    {
        [Required]
        public long InvoiceId { get; set; }

        [Required]
        public long ProdId { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public decimal Count { get; set; }

        public Product Product { get; set; }
        public Invoice Invoice { get; set; }
    }
}
