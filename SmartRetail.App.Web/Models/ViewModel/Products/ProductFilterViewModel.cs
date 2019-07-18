namespace SmartRetail.App.Web.Models.ViewModel.Products
{
    public class ProductFilterViewModel
    {
        public int? page { get; set; }
        public int? limit { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public int? shopId { get; set; }
    }
}
