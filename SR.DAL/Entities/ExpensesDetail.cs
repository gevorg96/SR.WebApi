using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("ExpenseDetail")]
    public class ExpensesDetail : Entity
    {
        [Required]
        public long ExpenseId { get; set; }

        [Required]
        public long ExpensesTypeId { get; set; }

        [Required]
        public decimal ExpenseDetailSum { get; set; }

        public virtual ExpensesType ExpensesType { get; set; }
    }
}
