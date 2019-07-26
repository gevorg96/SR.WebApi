using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRetail.App.DAL.Entities
{
    public class OrderDetails:IEntity
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int prod_id { get; set; }
        public decimal cost { get; set; }
        public decimal count { get; set; }

        public virtual Product Product { get; set; }
        public virtual Orders Order { get; set; }
    }
}
