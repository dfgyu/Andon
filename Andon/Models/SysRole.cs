// <summary>
// 角色实体类
// 有关角色的权限
// </summary>


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Andon.Models
{

    [Table("sys_role")]
    public class SysRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("role_name")]
        [MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(200)]
        [Column("description")]
        public string Description { get; set; }

        [Column("status")]
        public int Status { get; set; } = 0; // 0:正常, 1:禁止使用 默认为零
    }
}
