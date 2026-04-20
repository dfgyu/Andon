using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Andon.Models
{
    [Table("sys_user")]
    public class SysUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("email")]
        [MaxLength(50)]
        public string Email { get; set; }
        
        [Required]
        [Column("password")]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [Column("real_name")]
        [MaxLength(50)]
        public string RealName { get; set; }


        [Column("gender")]
        public int? Gender { get; set; }

        [Column("phone")]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public SysRole? Role { get; set; }

        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = true;
    }
}
