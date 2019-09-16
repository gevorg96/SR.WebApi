using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class OrderDetail:IEntity
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int prod_id { get; set; }
        public decimal cost { get; set; }
        public decimal count { get; set; }

        [Write(false)]
        [Computed]
        public virtual Product Product { get; set; }

        [Write(false)]
        [Computed]
        public virtual Order Order { get; set; }
    }
}
