namespace Andon.Dtos
{
    /// <summary>
    /// 表示用于物料搜索的条件 DTO（数据传输对象）。
    /// </summary>
    /// <remarks>
    /// 该对象用于在列表或分页查询中传递搜索条件与分页信息。
    /// 所有字符串属性均为可选，若为 <c>null</c> 或空字符串则表示不按该字段过滤。
    /// 分页属性有默认值：<see cref="Page"/> 默认为 1，<see cref="Limit"/> 默认为 10。
    /// 建议在接收端（服务/控制器）对分页参数进行边界校验（例如确保 Page >= 1，Limit 在合理范围内）。
    /// </remarks>
    public class MaterialSearchDto
    {
        /// <summary>
        /// 物料编码，用于按物料编码进行搜索。
        /// </summary>
        /// <example>"ABC123"</example>
        public string? MaterialCode { get; set; }

        /// <summary>
        /// 物料名称，用于按名称进行搜索。
        /// </summary>
        /// <example>"螺丝"</example>
        public string? MaterialName { get; set; }

        /// <summary>
        /// 物料类型，用于按类型过滤（例如：原材料、半成品、成品等）。
        /// </summary>
        /// <example>"原材料"</example>
        public string? Type { get; set; }

        /// <summary>
        /// 分页页码（从 1 开始）。
        /// </summary>
        /// <remarks>
        /// 默认值为 1。接收端应确保该值最小为 1。
        /// </remarks>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页返回的记录数。
        /// </summary>
        /// <remarks>
        /// 默认值为 10。接收端应对该值进行边界检查（例如最小为 1，最大可根据系统限制设置）。
        /// </remarks>
        public int Limit { get; set; } = 10;
    }
}
