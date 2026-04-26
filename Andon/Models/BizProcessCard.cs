using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Andon.Models
{
    [Table("biz_process_card")]
    public class BizProcessCard
    {
        [Key]
        public int Id { get; set; }

        [Column("card_code")]
        [MaxLength(50)]
        public string? CardCode { get; set; }

        [Column("process_name")]
        [MaxLength(100)]
        public string? ProcessName { get; set; }

        [Column("equipment_id")]
        public int? EquipmentId { get; set; }

        [Column("operator_id")]
        public int? OperatorId { get; set; }

        [Column("est_time")]
        public int? EstTime { get; set; }

        [Column("alarm_threshold")]
        public int? AlarmThreshold { get; set; }

        // 修复CS1061：添加导航属性
        public BizEquipment? BizEquipment { get; set; }
        public SysUser? SysUser { get; set; }
    }
}
