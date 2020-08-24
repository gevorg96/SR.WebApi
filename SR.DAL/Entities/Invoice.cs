using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Invoice")]
    public class Invoice : Entity
    {
        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        public long ShopId { get; set; }

        [Required]
        public bool IsInvoice { get; set; }

        public IList<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
