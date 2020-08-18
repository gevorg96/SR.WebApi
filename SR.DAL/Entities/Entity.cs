using System.ComponentModel.DataAnnotations;

namespace SR.DAL.Entities
{
    public abstract class Entity
    {
        [Key]
        public long Id { get; set; }
    }
}
