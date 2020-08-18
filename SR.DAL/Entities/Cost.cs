using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Cost")]
    public class Cost : Entity
    {
        [Required]
        public long ProdId { get; set; }
        public decimal? CostValue { get; set; }

        public Product Product { get; set; }
        public IList<Order> Orders { get; set; }
    }
}
