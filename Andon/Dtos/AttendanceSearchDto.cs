using System;
using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 查询考勤 DTO
    /// </summary>
    public class AttendanceSearchDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 工作日期
        /// </summary>
        public string? WorkDate { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页条数
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}
