using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 表示用于更新质量记录的传输对象（DTO）。
    /// 该 DTO 的所有属性均为可空以支持部分更新：只有非空属性会被视为需要更新的字段。
    /// </summary>
    public class QualityUpdateDto
    {
        /// <summary>
        /// 产品名称。可空，最大长度为 100 字符。
        /// 在部分更新场景中，若为 <c>null</c> 则表示不更新该字段。
        /// </summary>
        [MaxLength(100)]
        public string? ProductName { get; set; }

        /// <summary>
        /// 操作员标识符。可空。
        /// 在部分更新场景中，若为 <c>null</c> 则表示不更新操作员信息。
        /// </summary>
        public int? OperatorId { get; set; }

        /// <summary>
        /// 总数量。可空。
        /// 在部分更新场景中，若为 <c>null</c> 则表示不更新该值。
        /// </summary>
        public int? TotalQty { get; set; }

        /// <summary>
        /// 合格数量。可空。
        /// 在部分更新场景中，若为 <c>null</c> 则表示不更新该值。
        /// </summary>
        public int? QualifiedQty { get; set; }

        /// <summary>
        /// 不合格数量。可空。
        /// 在部分更新场景中，若为 <c>null</c> 则表示不更新该值。
        /// </summary>
        public int? UnqualifiedQty { get; set; }

        /// <summary>
        /// 是否合格标志。可空，通常按项目约定使用例如 0 表示不合格、1 表示合格。
        /// 在部分更新场景中，若为 <c>null</c> 则表示不更新该标志。
        /// </summary>
        public int? IsQualified { get; set; }

        /// <summary>
        /// 检测时间
        /// </summary>
        public DateTime? DetectionDate { get; set; }
    }
}