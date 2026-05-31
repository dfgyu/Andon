namespace Andon.Dtos
{
    /// <summary>
    /// 告警搜索参数传输对象（DTO）。
    /// 用于封装前端或调用方传递的告警筛选与分页参数。
    /// </summary>
    public class AlarmSearchDto
    {
        /// <summary>
        /// 产线标识（LineId）。
        /// 可为空；若为空则不按产线过滤。
        /// 示例： "LINE-01"。
        /// </summary>
        public string? LineId { get; set; }

        public int? EquipmentId { get; set; }


        /// <summary>
        /// 是否为停线告警标识（IsStopLine）。
        /// 可为空；常用值：
        ///  - 1：是停线告警
        ///  - 0：非停线告警
        /// 若为空则不按是否停线过滤。
        /// </summary>
        public int? IsStopLine { get; set; }

        public string? EquipmentName { get; set; }

        public int? Status { get; set; }

        public string? LevelCode { get; set; }

        /// <summary>
        /// 页码（从 1 开始）。
        /// 默认为 1。请确保调用方传入的值 >= 1。
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页记录数限制（Limit）。
        /// 默认为 10。可在业务层或接口层对最大值进行限制（例如 100）。
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}