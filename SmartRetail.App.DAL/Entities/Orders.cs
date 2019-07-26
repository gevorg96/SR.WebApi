using System;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{   
    public class Orders: IEntity
    {
        public int id { get; set; }
        public DateTime report_date { get; set; }
        public int shop_id { get; set; }    
        public bool isOrder { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
    }
}
