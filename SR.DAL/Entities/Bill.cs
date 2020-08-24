using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Bill")]
    public class Bill : Entity
    {
        [Required]
        public long ShopId { get; set; }

        [Required]
        public int BillNumber { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        public decimal BillSum { get; set; }

        [ForeignKey("ShopId")]
        public Shop Shop { get; set; }
        
        public IList<Sale> Sales { get; set; }
    }
}
