using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{   
    public class ExpensesType : IEntity
    {
        public ExpensesType()
        {
            this.Expenses = new HashSet<Expenses>();
        }
    
        public int id { get; set; }
        public string type { get; set; }
    
        public virtual ICollection<Expenses> Expenses { get; set; }
    }
}
