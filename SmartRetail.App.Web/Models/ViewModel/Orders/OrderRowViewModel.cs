namespace SmartRetail.App.Web.Models.ViewModel.Orders
{
    public class OrderRowViewModel
    {
        public int id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string vendorCode { get; set; }
        public decimal count { get; set; }
        public decimal? price { get; set; }
        public decimal? totalPrice { get; set; }
    }
}
