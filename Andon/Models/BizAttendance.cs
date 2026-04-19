using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Andon.Models
{
    [Table("biz_attendance")]
    public class BizAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("work_date")]
        [MaxLength(20)]
        public string WorkDate { get; set; }

        [Column("work_hours")]
        public int? WorkHours { get; set; }

        [Column("remark")]
        [MaxLength(255)]
        public string? Remark { get; set; } = null;
    }
}
