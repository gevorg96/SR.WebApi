using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Expense")]
    public class Expense : Entity
    {
        [Required]
        public long BusinessId { get; set; }

        [Required]
        public long ShopId { get; set; }

        [Required]
        public decimal ExpenseSum { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        public Business Business { get; set; }
        public IList<ExpensesDetail> ExpensesDetails { get; set; }
    }
}
