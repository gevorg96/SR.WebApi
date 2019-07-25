using System;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{
    public class Bills: IEntity
    {
        public int id { get; set; }
        public int shop_id { get; set; }
        public int bill_number { get; set; }
        public DateTime report_date { get; set; }
        public decimal sum { get; set; }
        public virtual List<Sales> Sales { get; set; }
    }
}
