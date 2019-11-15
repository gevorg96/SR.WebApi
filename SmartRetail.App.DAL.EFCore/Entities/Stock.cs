namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Stock
    {
        public int id { get; set; }
        public int? shop_id { get; set; }
        public int prod_id { get; set; }
        public decimal? count { get; set; }
    }
}