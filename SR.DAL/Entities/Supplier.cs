using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Supplier")]
    public class Supplier : Entity
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string OrgName { get; set; }

        [Required]
        public string Address { get; set; }
        public long? Tel { get; set; }

        public IList<Product> Product { get; set; }
    }
}
