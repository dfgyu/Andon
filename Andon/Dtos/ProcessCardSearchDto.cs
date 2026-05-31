namespace Andon.Dtos
{
    /// <summary>
    /// 用于承载流程卡查询条件与分页信息的 DTO。
    /// 包含可选的筛选字段以及用于分页的 Page/Limit。
    /// </summary>
    public class ProcessCardSearchDto
    {
        /// <summary>
        /// 流程卡编码。
        /// 可为空；当提供时用于按流程卡编码进行查询（可用于精确或模糊匹配，视上层实现而定）。
        /// </summary>
        public string? CardCode { get; set; }

        /// <summary>
        /// 工序名称。
        /// 可为空；当提供时用于按工序名称进行查询（可用于精确或模糊匹配，视上层实现而定）。
        /// </summary>
        public string? ProcessName { get; set; }

        /// <summary>
        /// 设备标识（ID）。
        /// 可为空；当提供时用于按设备进行过滤。
        /// </summary>
        public int? EquipmentId { get; set; }

        /// <summary>
        /// 操作员标识（ID）。
        /// 可为空；当提供时用于按操作员进行过滤。
        /// </summary>
        public int? OperatorId { get; set; }

        /// <summary>
        /// 当前页码（从 1 开始）。
        /// 默认为 1。建议值范围：>= 1。
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页条目数。
        /// 默认为 10。建议根据业务需要设置合理上限以防止一次请求返回过多数据（例如 100 或更小）。
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}
