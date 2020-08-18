using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("ExpenseType")]
    public class ExpensesType : Entity
    {
        [Required]
        public string Type { get; set; }
    }
}
