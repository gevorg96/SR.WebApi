﻿namespace SmartRetail.App.DAL.Entities
{
    public class OrderStock : IEntity
    {
        public int id { get; set; }
        public int? order_id { get; set; }
        public int? prod_id { get; set; }
        public decimal? curr_stock { get; set; }
    }
}
