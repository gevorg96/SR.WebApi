using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class Supplier: IEntity
    {
        public Supplier()
        {
            this.Product = new HashSet<Product>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string org_name { get; set; }
        public string supp_address { get; set; }
        public long? tel { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Product> Product { get; set; }
    }
}
