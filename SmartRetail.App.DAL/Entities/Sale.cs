using System;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{  
    public class Sale : IEntity
    {
        public int id { get; set; }
        public int prod_id { get; set; }
        public int bill_id { get; set; }
        public decimal sum { get; set; }
        public decimal count { get; set; }
        public int? unit_id { get; set; }
        public decimal cost { get; set; }
        public decimal profit { get; set; }
        public decimal price { get; set; }

        [Write(false)]
        [Computed]
        public virtual Product Product { get; set; }

        [Write(false)]
        [Computed]
        public virtual Unit Units { get; set; }
    }
}
