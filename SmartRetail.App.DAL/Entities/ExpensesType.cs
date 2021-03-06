using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{   
    public class ExpensesType : IEntity
    {
        public ExpensesType()
        {
            this.Expenses = new HashSet<Expense>();
        }
    
        public int id { get; set; }
        public string type { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
