using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{   
    public class Unit : IEntity
    {
        public Unit()
        {
            this.Sales = new HashSet<Sale>();
        }
    
        public int id { get; set; }
        public string value { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
