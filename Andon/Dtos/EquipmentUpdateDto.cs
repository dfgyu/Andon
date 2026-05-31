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

        [MaxLength(50)]
        public string? EquipmentModel { get; set; }

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
        /// 原料接收, //0<br/>
        /// 清理去杂, //1<br/>
        /// 磁选, //2<br/>
        /// 去石, //3<br/>
        /// 砻谷, //4<br/>
        /// 碾米, //5<br/>
        /// 抛光, //6<br/>
        /// 色选, //7<br/>
        /// 分级, //8<br/>
        /// 包装, //9<br/>
        /// 其他 //10<br/>
        /// </summary>
        public EquipmentsProcess Process { get; set; }

        /// <summary>           
        /// 设备状态，枚举类型，表示设备的当前状态 
        ///        运行,//0<br/>
        ///        待机,//1<br/>
        ///        故障,//2<br/>
        ///        维护,//3<br/>
        ///        离线//4<br/>
        /// </summary>
        public EquipmentStatus Status { get; set; }
        /// <summary>
        /// 报警联系人，长度不超过50个字符，表示当设备发生异常时的联系人信息
        /// </summary>
        public string? AlertContact { get; set; }

        /// <summary>
        /// 安装日期
        /// </summary>
       public DateTime? InstallationDate { get; set; }
        /// <summary>
        /// 维护日期
        /// </summary>
        public DateTime? MaintenanceDate { get; set; }
    }
}
