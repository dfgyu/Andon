using Andon.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Andon.Dtos
{
    /// <summary>
    ///  设备更新DTO类，用于接收前端传来的设备更新请求数据
    /// </summary>
    public class EquipmentUpdateDto
    {
        /// <summary>
        /// 设备编码，唯一标识设备，长度不超过50个字符
        /// </summary>
        [MaxLength(50)]
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// 设备名称，长度不超过100个字符
        /// </summary>
        [MaxLength(100)]
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 生产线ID，长度不超过20个字符
        /// </summary>  
        [MaxLength(20)]
        public string? LineId { get; set; }

        /// <summary>
        /// 设备所属工序，枚举类型
        /// </summary>
        public EquipmentsProcess Process { get; set; }

        /// <summary>           
        /// 设备状态，枚举类型，表示设备的当前状态 
        /// </summary>
        public EquipmentStatus Status { get; set; }

        public string? AlertContact { get; set; }
    }
}
