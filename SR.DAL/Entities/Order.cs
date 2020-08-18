using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Order")]
    public class Order : Entity
    {
        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        public long ShopId { get; set; }

        [Required]
        public bool IsOrder { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; }
    }
}
