using System;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{
    public class Cost: IEntity
    {
        public Cost()
        {
            this.Orders = new HashSet<Orders>();
        }
    
        public int id { get; set; }
        public int prod_id { get; set; }
        public decimal? value { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
