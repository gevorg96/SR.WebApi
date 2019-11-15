using System;

namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Order
    {
        public int id { get; set; }
        public DateTime report_date { get; set; }
        public bool isOrder { get; set; }
        
        public virtual Shop Shop { get; set; }
    }
}