using System;

namespace SmartRetail.App.DAL.Entities
{ 
    public class Expenses : IEntity
    {
        public int id { get; set; }
        public int business_id { get; set; }
        public int shop_id { get; set; }
        public int type_id { get; set; }
        public decimal value { get; set; }
        public DateTime report_date { get; set; }
    
        public virtual Business Business { get; set; }
        public virtual ExpensesType ExpensesType { get; set; }
    }
}
