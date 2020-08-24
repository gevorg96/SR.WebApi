using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace SR.DAL.Entities.Auth
{
    [Table("UserRole")]
    public class UserRole
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public JsonDocument Context { get; set; }

        public UserProfile User { get; set; }
        public Role Role { get; set; }
    }
}
