using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class Cost: IEntity
    {
        public Cost()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int id { get; set; }
        public int prod_id { get; set; }
        public decimal? value { get; set; }

        [Write(false)]
        [Computed]
        public virtual Product Product { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
