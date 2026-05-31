using System.ComponentModel.DataAnnotations;    

namespace Andon.Dtos
{
    /// <summary>
    /// 用于创建流程卡（Process Card）的数据传输对象（DTO）。
    /// </summary>
    /// <remarks>
    /// 该 DTO 在创建流程卡时从客户端接收必要信息，包含用于服务器端验证的 DataAnnotations。
    /// </remarks>
    public class ProcessCardCreateDto
    {
        /// <summary>
        /// 流程卡编码，唯一标识流程卡。
        /// </summary>
        /// <remarks>
        /// 必填项；最大长度为 50 个字符。应使用有意义且唯一的编码便于追溯。
        /// </remarks>
        [Required]
        [MaxLength(50)]
        public string? CardCode { get; set; }

        /// <summary>
        /// 工序名称，用于描述该流程卡对应的工序或步骤。
        /// </summary>
        /// <remarks>
        /// 必填项；最大长度为 100 个字符。
        /// </remarks>
        [Required]
        [MaxLength(100)]
        public string? ProcessName { get; set; }

        /// <summary>
        /// 关联的设备 Id（可选）。
        /// </summary>
        /// <remarks>
        /// 若指定，应对应系统中存在的设备记录；为空表示未指定设备。
        /// </remarks>
        public int? EquipmentId { get; set; }

        /// <summary>
        /// 关联的操作员 Id（可选）。
        /// </summary>
        /// <remarks>
        /// 若指定，应对应系统中存在的操作员/员工记录；为空表示未指定操作员。
        /// </remarks>
        public int? OperatorId { get; set; }

        /// <summary>
        /// 预计耗时（以分钟为单位，单位:min）。
        /// </summary>
        /// <remarks>
        /// 可选项，表示完成该工序的预计时间；可用于排程和估算产能。
        /// </remarks>
        public int? EstTime { get; set; }
    }
}
