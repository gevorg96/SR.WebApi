using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{   
    public class Units : IEntity
    {
        public Units()
        {
            this.Sales = new HashSet<Sales>();
        }
    
        public int id { get; set; }
        public string value { get; set; }
    
        public virtual ICollection<Sales> Sales { get; set; }
    }
}
