namespace SmartRetail.App.Web.Models.ViewModel
{
    public class ProductDetailViewModel
    {
        public int? Id { get; set; }
        public string ProdName { get; set; }
        public int ShopId { get; set; }
        public string VendorCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Stock { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int? UnitId { get; set; }
        public string ImgBase64 {get;set;}
        public string Category { get; set; }
    }
}
