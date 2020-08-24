using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SR.DAL.Entities.Auth
{
    [Table("RolePermission")]
    public class RolePermission
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }

    }
}
