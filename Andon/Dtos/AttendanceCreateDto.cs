using System.ComponentModel.DataAnnotations;    

namespace Andon.Dtos
{
    /// <summary>
    /// 新增考勤/打卡 DTO
    /// </summary>
    public class AttendanceCreateDto
    {
        /// <summary>
        /// 工作日期
        /// </summary>
        [Required(ErrorMessage = "工作日期不能为空")]
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
