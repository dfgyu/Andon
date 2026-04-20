using System;
using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 编辑考勤 DTO
    /// </summary>
    public class AttendanceUpdateDto
    {
        /// <summary>
        /// 工作日期
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string WorkDate { get; set; } = string.Empty;

        /// <summary>
        /// 工作时长
        /// </summary>
        public int? WorkHours { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(255)]
        public string? Remark { get; set; }
    }
}
