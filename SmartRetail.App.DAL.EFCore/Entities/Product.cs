namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public string attr1 { get; set; }
        public string attr2 { get; set; }
        public string attr3 { get; set; }
        public string attr4 { get; set; }
        public string attr5 { get; set; }
        public string attr6 { get; set; }
        public string attr7 { get; set; }
        public string attr8 { get; set; }
        public string attr9 { get; set; }
        public string attr10 { get; set; }
        
        public virtual Business Business { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual Folder Folder { get; set; }
        
    }
}