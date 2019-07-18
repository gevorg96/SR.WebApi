namespace SmartRetail.App.Web.Models
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
        public int UnitId { get; set; }
    }
}
