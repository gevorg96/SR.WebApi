using System;

namespace SmartRetail.App.DAL.Entities
{  
    public class Sales : IEntity
    {
        public int id { get; set; }
        public int prod_id { get; set; }
        public int bill_id { get; set; }
        public decimal sum { get; set; }
        public decimal count { get; set; }
        public int? unit_id { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Units Units { get; set; }
    }
}
