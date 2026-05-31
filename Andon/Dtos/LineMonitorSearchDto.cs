namespace Andon.Dtos
{
    public class LineMonitorSearchDto
    {
        /// <summary>
        /// 产线id。
        /// </summary>
        public string? LineId { get; set; }

        /// <summary>
        /// 设备id。
        /// </summary>
        public int? EquipmentId { get; set; }

        /// <summary>
        /// 是否报警状态。true表示报警，false表示正常，null表示不区分报警状态（查询所有）。
        /// </summary>
        public bool? IsAlarm { get; set; }

        /// <summary>
        /// 分页参数：页码和每页记录数。
        /// </summary>

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 200; // 大屏默认一次拉取

    }
}
