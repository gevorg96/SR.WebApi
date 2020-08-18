using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{ 
    public class Expense : IEntity
    {
        public int id { get; set; }
        public int business_id { get; set; }
        public int shop_id { get; set; }
        public decimal sum { get; set; }
        public DateTime report_date { get; set; }

        [Write(false)]
        [Computed]
        public virtual Business Business { get; set; }

        [Write(false)]
        [Computed]
        public virtual List<ExpensesDetail> ExpensesDetails { get; set; }

        public Expense()
        {
            ExpensesDetails = new List<ExpensesDetail>();
        }
    }
}
