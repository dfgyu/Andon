namespace Andon.Dtos
{
    /// <summary>
    /// 硬件报警数据传输对象（DTO）。
    /// 表示来自产线的灯光/声音报警及停线标志，用于在系统各层或服务间传递报警信息。
    /// </summary>
    public class HardwareAlarmDto
    {
        /// <summary>
        /// 产线标识（例如产线编号或 ID）。
        /// 可能为 <c>null</c>，表示产线未知或未指定。
        /// </summary>
        public string? LineId { get; set; }

        /// <summary>
        /// 灯光报警信息或类型（例如 "红灯"、"黄灯"、"闪烁" 或自定义描述）。
        /// 可能为 <c>null</c>，表示当前无灯光报警。
        /// </summary>
        public string? LightAlarm { get; set; }

        /// <summary>
        /// 声音报警信息或类型（例如 "蜂鸣"、"语音提示" 或自定义描述）。
        /// 可能为 <c>null</c>，表示当前无声音报警。
        /// </summary>
        public string? SoundAlarm { get; set; }

        /// <summary>
        /// 停线标志。
        /// 约定取值：0 表示未停线，1 表示已停线。
        /// </summary>
        public int IsStopLine { get; set; }
    }
}