using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SmartRetail.App.Web.Models.ViewModel.Products
{
    public class ProductDetailViewModel
    {
        public int? Id { get; set; }
        public string ProdName { get; set; }
        public string VendorCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int? UnitId { get; set; }
        public string ImgBase64 {get;set;}
        public IFormFile img { get; set; }
        public string Category { get; set; }
        public List<ShopStockViewModel> Stocks { get; set; }
    }
}
