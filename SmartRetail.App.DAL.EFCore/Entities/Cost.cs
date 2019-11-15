namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Cost
    {
        public int id { get; set; }
        public decimal? value { get; set; }
        
        public virtual Product Product { get; set; }
    }
}