using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 表示用于创建工具的传输对象（DTO）。
    /// 用于接收客户端提交的创建工具请求数据。
    /// </summary>
    public class ToolCreateDto
    {
        /// <summary>
        /// 工具名称。
        /// </summary>
        /// <remarks>
        /// 最大长度为 50 字符。该字段可为 null，表示名称未知或不提供。
        /// 对应的数据注解：<see cref="MaxLengthAttribute"/> (50)。
        /// </remarks>
        [MaxLength(50)]
        public string? ToolName { get; set; }

        /// <summary>
        /// 工具型号。
        /// </summary>
        /// <remarks>
        /// 最大长度为 50 字符。该字段可为 null。
        /// 对应的数据注解：<see cref="MaxLengthAttribute"/> (50)。
        /// </remarks>
        [MaxLength(50)]
        public string? ToolModel { get; set; }

        /// <summary>
        /// 工具总数量。
        /// </summary>
        /// <remarks>
        /// 可为空的整型；当数量未知时可使用 null 表示。
        /// </remarks>
        public int? TotalQty { get; set; }

        /// <summary>
        /// 剩余数量（当前库存可用数量）。
        /// </summary>
        /// <remarks>
        /// 默认为 0。该属性不为 null，以便在业务逻辑中安全使用数值运算。
        /// </remarks>
        public int SurplusQty { get; set; } = 0;

        /// <summary>
        /// 仓库标识或名称。
        /// </summary>
        /// <remarks>
        /// 最大长度为 20 字符。该字段可为 null，表示未指定仓库。
        /// 对应的数据注解：<see cref="MaxLengthAttribute"/> (20)。
        /// </remarks>
        [MaxLength(20)]
        public string? Warehouse { get; set; }

        /// <summary>
        /// 维护日期。
        /// </summary>
        public DateTime? MaintenanceDate { get; set; }

        /// <summary>
        /// 安全库存。
        /// </summary>
        public int? SafetyMargin { get; set; }
    }
}