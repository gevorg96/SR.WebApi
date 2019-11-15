namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Price
    {
        public int id { get; set; }
        public decimal? price { get; set; }
        
        public virtual Product Product { get; set; }
        public virtual Shop Shop { get; set; }
    }
}