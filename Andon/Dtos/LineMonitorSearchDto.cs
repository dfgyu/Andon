namespace Andon.Dtos
{
    public class LineMonitorSearchDto
    {
        public string? LineId { get; set; }
        public int? EquipmentId { get; set; }
        public bool? IsAlarm { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 200; // 大屏默认一次拉取

    }
}
