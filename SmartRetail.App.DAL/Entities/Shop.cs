using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{
    public class Shop : IEntity
    {
        public Shop()
        { }
    
        public int id { get; set; }
        public string shop_address { get; set; }
        public string name { get; set; }
        public int? business_id { get; set; }

        public virtual ICollection<Business> Businesses { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<Bills> Bills { get; set; }
        public virtual ICollection<Stock> Stock { get; set; }
        public virtual ICollection<UserProfile> Users { get; set; }
        public virtual Business Business { get; set; }
    }
}
