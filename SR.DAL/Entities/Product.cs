using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SR.DAL.Entities
{
    [Table("Product")]
    public class Product : Entity
    {
        [Required]
        public long BusinessId { get; set; }

        [Required]
        public long[] ShopIds { get; set; }

        [Required]
        public long SupplierId { get; set; }

        [Required]
        public string ProductName { get; set; }
        public string Attr1 { get; set; }
        public string Attr2 { get; set; }
        public string Attr3 { get; set; }
        public string Attr4 { get; set; }
        public string Attr5 { get; set; }
        public string Attr6 { get; set; }
        public string Attr7 { get; set; }
        public string Attr8 { get; set; }
        public string Attr9 { get; set; }
        public string Attr10 { get; set; }
        public long? UnitId { get; set; }
        public long? FolderId { get; set; }

        [NotMapped]
        public string Category { get; set; }

        [NotMapped]
        public string ImgBase64 { get; set; }

        public IList<Cost> Cost { get; set; }      
        public IList<Price> Prices { get; set; }
        public Business Business { get; set; }
        public Supplier Supplier { get; set; }
        public IList<Stock> Stock { get; set; }
        public Price Price { get; set; }
        public Image Image { get; set; }
        public Unit Unit { get; set; }
        public MemoryStream ImgMemoryStream { get; set; }
    }
}
