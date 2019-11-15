namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Supplier
    {
        public int id { get; set; }
        public string name { get; set; }
        public string org_name { get; set; }
        public string supp_address { get; set; }
        public long? tel { get; set; }
    }
}