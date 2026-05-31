using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    /// <summary>
    /// 用于更新物料信息的传输对象（DTO）。
    /// 包含可更新的物料字段，用于 API 或服务层的更新操作。
    /// </summary>
    public class MaterialUpdateDto
    {
        /// <summary>
        /// 物料编码。
        /// 最大长度为 50，可为空（null 表示不更新该字段）。
        /// </summary>
        [MaxLength(50)]
        public string? MaterialCode { get; set; }



        /// <summary>
        /// 物料名称。
        /// 最大长度为 100，可为空（null 表示不更新该字段）。
        /// </summary>
        [MaxLength(100)]
        public string? MaterialName { get; set; }

        /// <summary>
        /// 物料类别或型号。
        /// 最大长度为 20，可为空（null 表示不更新该字段）。
        /// </summary>
        [MaxLength(20)]
        public string? Type { get; set; }

        /// <summary>
        /// 剩余数量。
        /// 使用整型表示，默认为 0；如果不希望更新该值，请在调用端忽略或使用约定的更新策略。
        /// </summary>
        public int SurplusQty { get; set; }

        /// <summary>
        /// 安全库存量。
        /// 默认为 0；如果不希望更新该值，请在调用端忽略或使用约定的更新策略。
        /// </summary>
        public int SafetyMargin { get; set; }
    }
}