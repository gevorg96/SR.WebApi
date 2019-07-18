namespace SmartRetail.App.DAL.Entities
{
    public class Stock : IEntity
    {
        public int id { get; set; }
        public int? shop_id { get; set; }
        public int prod_id { get; set; }
        public decimal? count { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
