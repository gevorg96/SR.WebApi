namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Sale
    {
        public int id { get; set; }
        public decimal sum { get; set; }
        public decimal count { get; set; }
        public decimal cost { get; set; }
        public decimal profit { get; set; }
        public decimal price { get; set; }
        
        public virtual Product Product { get; set; }
        public virtual Bill Bill { get; set; }
        public virtual Unit Unit { get; set; }
        
    }
}