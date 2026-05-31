namespace Andon.Dtos
{
    /// <summary>
    /// 工具搜索参数的数据传输对象（DTO）。
    /// </summary>
    /// <remarks>
    /// 用于封装前端或调用方传入的工具筛选条件和分页信息。
    /// - 字符串筛选字段为可空，若为 <c>null</c> 或空字符串则忽略该筛选条件。
    /// - 页码从 1 开始，默认值为 1。建议在调用端确保为正整数。
    /// - 每页条目数默认为 10，建议对其范围进行上限限制以防止过大请求。
    /// </remarks>
        public class ToolSearchDto
        {
            /// <summary>
            /// 要匹配的工具名称。为可选字段，支持部分匹配（例如 SQL 的 LIKE）。
            /// 若为 <c>null</c> 或空，则不按名称过滤。
            /// </summary>
            public string? ToolName { get; set; }

            /// <summary>
            /// 要匹配的工具型号。为可选字段，支持部分匹配。
            /// 若为 <c>null</c> 或空，则不按型号过滤。
            /// </summary>
            public string? ToolModel { get; set; }

            /// <summary>
            /// 仓库或库位标识，用于限定查询的仓库范围。为可选字段。
            /// 若为 <c>null</c> 或空，则不按仓库过滤。
            /// </summary>
            public string? Warehouse { get; set; }

            /// <summary>
            /// 当前页码（从 1 开始）。默认值为 1。
            /// 应保证为正整数（建议在请求层或验证层进行校验）。
            /// </summary>
            public int Page { get; set; } = 1;

            /// <summary>
            /// 每页返回的记录数。默认值为 10。
            /// 建议对该值进行上限约束（例如 100）以防止过大数据请求。
            /// </summary>
            public int Limit { get; set; } = 10;
    }
}