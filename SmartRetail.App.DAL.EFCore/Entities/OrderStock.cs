namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class OrderStock
    {
        public int id { get; set; }
        public decimal? curr_stocks { get; set; }
        
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual Product Product { get; set; }
        public virtual Shop Shop { get; set; }
    }
}