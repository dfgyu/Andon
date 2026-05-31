namespace Andon.Dtos
{
    /// <summary>
    /// 流程卡 数据传输对象 (DTO)。
    /// 用于在服务层与客户/前端之间传递流程卡相关数据。
    /// </summary>
            public class ProcessCardDto
            {
                /// <summary>
                /// 流程卡主键标识。
                /// </summary>
                public int Id { get; set; }

                /// <summary>
                /// 流程卡编码，可以为空。
                /// 示例：用于唯一标识流程卡的字符串编码。
                /// </summary>
                public string? CardCode { get; set; }

                /// <summary>
                /// 流程名称，可以为空。
                /// </summary>
                public string? ProcessName { get; set; }

                /// <summary>
                /// 关联的设备 Id，可为空。
                /// 当存在设备关联时，该字段指向设备表的主键。
                /// </summary>
                public int? EquipmentId { get; set; }

                /// <summary>
                /// 设备名称（关联查询得来），可以为空。
                /// 用于在 DTO 中直接展示设备的人类可读名称，而无需额外查询。
                /// </summary>
                public string? EquipmentName { get; set; } // 设备名称（关联查询）

                /// <summary>
                /// 操作员 Id，可为空。
                /// 当流程卡与操作员有关联时，该字段指向操作员表的主键。
                /// </summary>
                public int? OperatorId { get; set; }

                /// <summary>
                /// 操作员名称（关联查询得来），可以为空。
                /// 用于在 DTO 中直接展示操作员的人类可读名称。
                /// </summary>
                public string? OperatorName { get; set; } // 操作员名称（关联查询）

                /// <summary>
                /// 预计耗时（秒或分钟，视业务定义），可以为空。
                /// 请根据业务约定确认该字段单位（例如秒或分钟）。
                /// </summary>
                public int? EstTime { get; set; }
            }
}
