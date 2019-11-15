namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class Folder
    {
        public int id { get; set; }
        public string folder { get; set; }
        
        public virtual Business Business { get; set; }
        public virtual Folder Parent { get; set; }
    }
}