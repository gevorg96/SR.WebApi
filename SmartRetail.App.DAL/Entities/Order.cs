using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{   
    public class Order: IEntity
    {
        public int id { get; set; }
        public DateTime report_date { get; set; }
        public int shop_id { get; set; }    
        public bool isOrder { get; set; }

        [Write(false)]
        [Computed]
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}
