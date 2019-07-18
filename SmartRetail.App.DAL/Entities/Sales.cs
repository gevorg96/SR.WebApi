using System;

namespace SmartRetail.App.DAL.Entities
{  
    public class Sales : IEntity
    {
        public int id { get; set; }
        public int prod_id { get; set; }
        public int? shop_id { get; set; }
        public DateTime? report_date { get; set; }
        public int? bill_number { get; set; }
        public decimal? summ { get; set; }
        public decimal? sales_count { get; set; }
        public int? unit_id { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual Units Units { get; set; }
    }
}
