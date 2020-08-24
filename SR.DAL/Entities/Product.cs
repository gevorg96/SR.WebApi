using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SR.DAL.Entities
{
    [Table("Product")]
    public class Product : Entity
    {
        [Required]
        public long[] ShopIds { get; set; }

        [Required]
        public long[] SupplierIds { get; set; }

        [Required]
        public string Name { get; set; }

        public JsonDocument Attributes { get; set; }
        public long[] UnitIds { get; set; }
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
