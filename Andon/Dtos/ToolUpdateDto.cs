using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 表示用于更新工具信息的数据传输对象（DTO）。
    /// 用于接收客户端提交的可选或部分更新字段。
    /// </summary>
    public class ToolUpdateDto
    {
        /// <summary>
        /// 工具名称。
        /// 可空：允许不提供以表示不更新该字段。
        /// 最大长度：50。
        /// </summary>
        [MaxLength(50)]
        public string? ToolName { get; set; }

        /// <summary>
        /// 工具型号。
        /// 可空：允许不提供以表示不更新该字段。
        /// 最大长度：50。
        /// </summary>
        [MaxLength(50)]
        public string? ToolModel { get; set; }

        /// <summary>
        /// 工具总数量。
        /// 可空：null 表示不更新该字段；提供数值时以该值为准。
        /// </summary>
        public int? TotalQty { get; set; }

        /// <summary>
        /// 剩余数量（当前库存）。
        /// 非空：该字段为 int，表示若包含在更新请求中则必须提供整数值。
        /// </summary>
        public int SurplusQty { get; set; }

        /// <summary>
        /// 所在仓库名称或编码。
        /// 可空：允许不提供以表示不更新该字段。
        /// 最大长度：20。
        /// </summary>
        [MaxLength(20)]
        public string? Warehouse { get; set; }

        /// <summary>
        /// 维护日期。
        /// </summary>
        public DateTime? MaintenanceDate { get; set; }

        /// <summary>
        /// 安全库存量。
        /// </summary>
        public int? SafetyMargin { get; set; }
    }
}