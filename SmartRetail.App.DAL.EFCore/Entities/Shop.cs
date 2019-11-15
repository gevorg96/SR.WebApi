namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Shop
    {
        public int id { get; set; }
        public string shop_address { get; set; }
        public string name { get; set; }
        
        public virtual Business Business { get; set; }
    }
}