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
        ///  开始工作时间
        /// </summary>
        public TimeSpan? StartWorkTime { get; set; }

        /// <summary>
        /// 结束工作时间
        /// </summary>          
        public TimeSpan? EndWorkTime { get; set; }

        /// <summary>
        /// 考勤状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(255)]
        public string? Remark { get; set; }
    }
}
