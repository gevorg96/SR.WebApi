using System;

namespace SmartRetail.App.DAL.Entities
{   
    public class Orders: IEntity
    {
        public int id { get; set; }
        public int prod_id { get; set; }
        public decimal cost { get; set; }
        public decimal count { get; set; }
        public decimal stock_sale { get; set; }
        public DateTime report_date { get; set; }
        public int shop_id { get; set; }    

        public virtual Cost Cost { get; set; }
        public virtual Product Product { get; set; }
    }
}
