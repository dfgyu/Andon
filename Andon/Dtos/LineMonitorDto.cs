namespace Andon.Dtos
{
    public class LineMonitorDto
    {
        // 设备信息
        public int EquipmentId { get; set; }
        public string? EquipmentCode { get; set; }
        public string? EquipmentName { get; set; }
        public string? LineId { get; set; }

        // 工序
        public string? ProcessName { get; set; }

        // 工序卡 + 操作员
        public int? ProcessCardId { get; set; }
        public string? CardCode { get; set; }
        public string? OperatorName { get; set; }

        // 实时状态（IoT）
        public int RunStatus { get; set; }
        public string RunStatusText { get; set; } = "";

        // 异常标记
        public bool IsBlocked { get; set; }
        public bool IsOverheat { get; set; }
        public bool IsDeviation { get; set; }
        public bool IsPackError { get; set; }

        // 监控状态
        public bool IsAlarm { get; set; }
        public string? AlarmMessage { get; set; }

        // 采集时间
        public string? CollectionTime { get; set; }
    }
}
