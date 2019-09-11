namespace SmartRetail.App.Web.Models.ViewModel.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string ProdName { get; set; }
        public string VendorCode { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal Stock { get; set; }
        public string ImgUrl { get; set; }
        public string Vendor { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int? UnitId { get; set; }
        public int? CategoryId { get; set; }
    }
}
