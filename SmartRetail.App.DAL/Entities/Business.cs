using System;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Entities
{
    public class Business : IEntity
    {
        public Business()
        { }
    
        public int id { get; set; }
        public string name { get; set; }
        public long? tel { get; set; }
    
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
        public virtual ICollection<Shop> Shops { get; set; }
        public virtual ICollection<Expenses> Expenses { get; set; }
    }
}
