using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{
    public class Supplier
    {
        public Supplier()
        {
            this.Product = new HashSet<Product>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string org_name { get; set; }
        public string supp_address { get; set; }
        public int? tel { get; set; }
    
        public virtual ICollection<Product> Product { get; set; }
    }
}
