using System;

namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Bill
    {
        public int id { get; set; }
        public int bill_number { get; set; }
        public DateTime report_date { get; set; }
        public decimal sum { get; set; }
        
        public virtual Shop Shop { get; set; }
    }
}