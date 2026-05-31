using System.ComponentModel.DataAnnotations;    

namespace Andon.Dtos
{
    /// <summary>
    /// 新增考勤/打卡 DTO
    /// </summary>
    public class AttendanceCreateDto
    {
        /// <summary>
        /// 员工id
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// 工作日期
        /// </summary>
        [Required(ErrorMessage = "工作日期不能为空")]
        [MaxLength(20)]
        public string WorkDate { get; set; } = string.Empty;

        /// <summary>
        /// 开始工作时间
        /// </summary>
        public TimeSpan StartWorkTime { get; set; }

        /// <summary>
        /// 结束工作时间
        /// </summary>
        public TimeSpan EndWorkTime { get; set; }

        /// <summary>
        /// 考勤状态
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(255)]
        public string? Remark { get; set; }

    }
}
