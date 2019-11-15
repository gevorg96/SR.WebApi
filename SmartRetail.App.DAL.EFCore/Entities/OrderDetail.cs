namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class OrderDetail
    {
        public int id { get; set; }
        public decimal cost { get; set; }
        public decimal count { get; set; }
        
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}