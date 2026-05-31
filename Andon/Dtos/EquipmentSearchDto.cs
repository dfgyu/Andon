using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Andon.Enums;


namespace Andon.Dtos
{
    public class EquipmentSearchDto
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }

        public string? EquipmentModel { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 产线ID
        /// </summary>
        public string? LineId { get; set; }

        /// <summary>
        /// 设备状态（可空枚举筛选）
        ///        运行,//0<br/>
        ///        待机,//1<br/>
        ///        故障,//2<br/>
        ///        维护,//3<br/>
        ///        离线//4<br/>
        /// </summary>
        public EquipmentStatus? Status { get; set; }

        /// <summary>
        /// 设备工序（可空枚举筛选）
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
        public EquipmentsProcess? Process { get; set; }

        /// <summary>
        /// 报警联系人
        /// </summary>
        public string? AlertContact { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页数量
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}
