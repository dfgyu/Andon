using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 用于创建质量记录的传输对象（DTO）。
    /// 包含与产品质量统计相关的字段，用于接收来自客户端的创建请求。
    /// </summary>
    public class QualityCreateDto
    {
        /// <summary>
        /// 产品名称，最大长度 100 字符。
        /// 可为空：当产品名称不可用或不需要记录名称时可省略。
        /// </summary>
        [MaxLength(100)]
        public string? ProductName { get; set; }

        /// <summary>
        /// 操作员标识（用户 ID）。
        /// 可为空：当创建记录时不需要绑定具体操作员或由系统自动分配时可省略。
        /// </summary>
        public int? OperatorId { get; set; }

        /// <summary>
        /// 总数量（Total quantity）。
        /// 可为空：在某些场景下总数可能未知或不适用。
        /// 单位：件/个（根据业务约定）。
        /// </summary>
        public int? TotalQty { get; set; }

        /// <summary>
        /// 合格数量（Qualified quantity）。
        /// 可为空：在尚未完成检验或数据不完整时可省略。
        /// 单位：件/个。
        /// </summary>
        public int? QualifiedQty { get; set; }

        /// <summary>
        /// 不合格数量（Unqualified / defective quantity）。
        /// 可为空：在尚未完成检验或数据不完整时可省略。
        /// 单位：件/个。
        /// </summary>
        public int? UnqualifiedQty { get; set; }
        /// <summary>
        /// 检测日期（Detection date）。
        /// </summary>
        public DateTime? DetectionDate { get; set; }

        /// <summary>
        /// 是否合格标识：
        /// 0 - 不合格；1 - 合格。
        /// 可为空：当未确定是否合格时可省略。
        /// 使用整数以兼容数据库或现有枚举映射，必要时可改为枚举类型以增强可读性。
        /// </summary>
        public int? IsQualified { get; set; } // 0不合格 1合格
    }
}