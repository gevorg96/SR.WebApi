using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Cost")]
    public class Cost : Entity
    {
        [Required]
        public decimal? Value { get; set; }

        public IList<Invoice> Orders { get; set; }
    }
}
