namespace Andon.Dtos
{
    /// <summary>
    /// 用于质量记录查询的参数传输对象（DTO）。
    /// 包含用于筛选和分页的字段。
    /// </summary>
    public class QualitySearchDto
    {
        /// <summary>
        /// 产品名称，用于按产品名称进行模糊或精确匹配筛选。
        /// 如果为 <c>null</c> 或空字符串，则不按产品名称过滤。
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// 操作员的标识（ID），用于按操作员筛选。
        /// 如果为 <c>null</c>，则不按操作员过滤。
        /// </summary>
        public int? OperatorId { get; set; }

        /// <summary>
        /// 是否合格筛选。
        /// 约定值：<c>1</c> 表示合格，<c>0</c> 表示不合格，<c>null</c> 表示不过滤此条件。
        /// </summary>
        public int? IsQualified { get; set; } // 按合格/不合格筛选

        /// <summary>
        /// 页码（从 1 开始）。用于分页查询，默认值为 1。
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页记录数。用于分页查询，默认值为 10。
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}