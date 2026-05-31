
using System;

namespace Andon.Dtos
{
    /// <summary>
    /// 生产线监控数据传输对象（DTO）。
    /// 用于在服务、控制器与前端之间传递设备及其监控状态信息。
    /// </summary>
    public class LineMonitorDto
    {
        // 设备信息

        /// <summary>
        /// 设备标识（主键）。
        /// </summary>
        public int EquipmentId { get; set; }

        /// <summary>
        /// 设备编码（例如条码或唯一字符串标识）。
        /// 可为 null 表示编码未知或未填写。
        /// </summary>
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// 设备名称（人类可读）。
        /// 可为 null 表示名称未知或未配置。
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 所属产线标识。
        /// 可为 null 表示未分配产线。
        /// </summary>
        public string? LineId { get; set; }

        // 工序

        /// <summary>
        /// 当前工序名称。
        /// 可为 null 表示未配置或无活动工序。
        /// </summary>
        public string? ProcessName { get; set; }

        // 工序卡 + 操作员

        /// <summary>
        /// 当前使用的工序卡 ID，若无则为 null。
        /// </summary>
        public int? ProcessCardId { get; set; }

        /// <summary>
        /// 工序卡编码或编号。
        /// 可为 null 表示未分配卡片或未知。
        /// </summary>
        public string? CardCode { get; set; }

        /// <summary>
        /// 当前操作员姓名或标识。
        /// 可为 null 表示无人或未登录。
        /// </summary>
        public string? OperatorName { get; set; }

        // 实时状态（IoT）

        /// <summary>
        /// 运行状态代码（由 IoT 或状态机定义的整数值）。
        /// 例如：0=停止、1=运行、2=故障（具体映射由上层系统解释）。
        /// </summary>
        public int RunStatus { get; set; }

        /// <summary>
        /// 运行状态文本描述（用于展示）。
        /// 默认为空字符串以避免 null 引发的显示问题。
        /// </summary>
        public string RunStatusText { get; set; } = "";

        // 异常标记

        /// <summary>
        /// 是否堵塞/卡料。
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// 是否过热（超温）。
        /// </summary>
        public bool IsOverheat { get; set; }

        /// <summary>
        /// 是否偏差（例如定位或计量偏差）。
        /// </summary>
        public bool IsDeviation { get; set; }

        /// <summary>
        /// 是否包装错误（包装相关异常）。
        /// </summary>
        public bool IsPackError { get; set; }

        // 监控状态

        /// <summary>
        /// 是否处于报警状态（综合监控判断）。
        /// </summary>
        public bool IsAlarm { get; set; }

        /// <summary>
        /// 报警信息的文本描述，若无则为 null。
        /// 可包含多项报警原因的合并描述。
        /// </summary>
        public string? AlarmMessage { get; set; }

        // 采集时间

        /// <summary>
        /// 数据采集时间的字符串表示（建议使用 ISO 8601 格式，如 "2024-01-01T12:00:00Z"）。
        /// 可为 null 表示时间未知。
        /// </summary>
        public string? CollectionTime { get; set; }
    }
}
