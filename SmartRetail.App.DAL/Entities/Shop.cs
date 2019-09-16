using System.Collections.Generic;
using Dapper.Contrib.Extensions;

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

        [Write(false)]
        [Computed]
        public virtual ICollection<Business> Businesses { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Product> Product { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Bill> Bills { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Stock> Stock { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<UserProfile> Users { get; set; }

        [Write(false)]
        [Computed]
        public virtual Business Business { get; set; }
    }
}
