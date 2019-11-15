using System;

namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Expense
    {
        public int id { get; set; }
        public decimal sum { get; set; }
        public DateTime report_date { get; set; }
        
        public virtual Shop Shop { get; set; }
        public virtual Business Business { get; set; }
    }
}