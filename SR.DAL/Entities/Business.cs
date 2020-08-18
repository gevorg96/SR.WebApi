using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities
{
    [Table("Business")]
    public class Business : Entity
    {
        [Required]
        public string Name { get; set; }
        public long? Tel { get; set; }

        public IList<UserProfile> UserProfiles { get; set; }
        public IList<Shop> Shops { get; set; }
        public IList<Expense> Expenses { get; set; }
    }
}
