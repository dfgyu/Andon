using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 用于创建物料的传输对象。
    /// 包含用于创建新物料所需的基本字段及其验证约束。
    /// </summary>
    public class MaterialCreateDto
    {
        /// <summary>
        /// 物料编码。
        /// 最大长度为 50 个字符。可为空；如果需要唯一性或必填，请在上层验证中处理。
        /// </summary>
        [MaxLength(50)]
        public string? MaterialCode { get; set; }



        /// <summary>
        /// 物料名称。
        /// 最大长度为 100 个字符。可为空。
        /// </summary>
        [MaxLength(100)]
        public string? MaterialName { get; set; }

        /// <summary>
        /// 物料类型。
        /// 最大长度为 20 个字符。可用于分类、筛选或展示之用。
        /// </summary>
        [MaxLength(20)]
        public string? Type { get; set; }

        /// <summary>
        /// 剩余数量（库存或可用数量）。
        /// 默认为 0。请在需要时在上层逻辑中校验非负性或业务规则。
        /// </summary>
        public int SurplusQty { get; set; } = 0;

        /// <summary>
        /// 安全库存量。
        /// 默认为 0。请在需要时在上层逻辑中校验非负性或业务规则。
        /// </summary>
        public int SafetyMargin { get; set; } = 0;
    }
}