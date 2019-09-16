using System.Collections.Generic;
using System.IO;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class Product : IEntity
    {
        public Product()
        {
            this.Cost = new HashSet<Cost>();
            this.Orders = new HashSet<Order>();
            this.Prices = new HashSet<Price>();
            this.Sales = new HashSet<Sale>();
            this.Stock = new HashSet<Stock>();
        }
    
        [Key]
        public int id { get; set; }
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
    
        [Write(false)]
        [Computed]
        public virtual ICollection<Cost> Cost { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Order> Orders { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Price> Prices { get; set; }

        [Write(false)]
        [Computed]
        public virtual Business Business { get; set; }

        [Write(false)]
        [Computed]
        public virtual Supplier Supplier { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Sale> Sales { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Stock> Stock { get; set; }

        [Write(false)]
        [Computed]
        public virtual Price Price { get; set; }

        [Write(false)]
        [Computed]
        public virtual Image Image { get; set; }

        [Write(false)]
        [Computed]
        public virtual Unit Unit { get; set; }
        [Write(false)]
        [Computed]
        public virtual string ImgBase64 { get; set; }

        [Write(false)]
        [Computed]
        public virtual MemoryStream ImgMemoryStream { get; set; }

        [Write(false)]
        [Computed]
        public virtual string Category { get; set; }
        
    }
}
