using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class BillParent: IEntity
    {
        public int id { get; set; }
        public int shop_id { get; set; }
        public int bill_number { get; set; }
        public DateTime report_date { get; set; }
        public decimal sum { get; set; }

        [Write(false)]
        [Computed]
        public virtual List<Sale> Sales { get; set; }
    }
}
