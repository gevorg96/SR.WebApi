using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{
    public class Product : IEntity
    {
        public Product()
        {
            this.Cost = new HashSet<Cost>();
            this.Orders = new HashSet<Orders>();
            this.Prices = new HashSet<Price>();
            this.Sales = new HashSet<Sales>();
            this.Stock = new HashSet<Stock>();
        }
    
        public int id { get; set; }
        public int? shop_id { get; set; }
        public int? business_id { get; set; }
        public int? supplier_id { get; set; }
        public string name { get; set; }
        public string attr1 { get; set; }
        public string attr2 { get; set; }
        public string attr3 { get; set; }
        public string attr4 { get; set; }
        public string attr5 { get; set; }
        public string attr6 { get; set; }
        public string attr7 { get; set; }
        public string attr8 { get; set; }
        public string attr9 { get; set; }
        public string attr10 { get; set; }
        public int? unit_id { get; set; }
        public int? folder_id { get; set; }
    
        public virtual ICollection<Cost> Cost { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Price> Prices { get; set; }
        //public virtual Shop Shop { get; set; }
        public virtual Business Business { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Sales> Sales { get; set; }
        public virtual ICollection<Stock> Stock { get; set; }
        public virtual Price Price { get; set; }
        public virtual Images Image { get; set; }
        public virtual Units Unit { get; set; }
    }
}
