using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace SmartRetail.App.DAL.Entities
{
    public class Business : IEntity
    {
        public Business()
        { }
    
        public int id { get; set; }
        public string name { get; set; }
        public long? tel { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<UserProfile> UserProfiles { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Shop> Shops { get; set; }

        [Write(false)]
        [Computed]
        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
