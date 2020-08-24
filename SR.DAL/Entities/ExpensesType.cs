using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("ExpenseType")]
    public class ExpenseType : Entity
    {
        [Required]
        public string Name { get; set; }
    }
}
