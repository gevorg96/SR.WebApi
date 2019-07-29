using System;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{ 
    public class Expenses : IEntity
    {
        public int id { get; set; }
        public int business_id { get; set; }
        public int? shop_id { get; set; }
        public decimal sum { get; set; }
        public DateTime report_date { get; set; }
    
        public virtual Business Business { get; set; }
        public virtual List<ExpensesDetails> ExpensesDetails { get; set; }

        public Expenses()
        {
            ExpensesDetails = new List<ExpensesDetails>();
        }
    }
}
