using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Andon.Enums;

namespace Andon.Models
{
    [Table("biz_iot_equipment_data")]
    public class BizIotEquipmentData
    {
        [Key]
        public int Id { get; set; }

        [Column("line_id")]
        [MaxLength(50)]
        public string LineId { get; set; } = "";

        [Column("equipment_id")]
        public int EquipmentId { get; set; }

        [Column("run_status")]
        public EquipmentStatus RunStatus { get; set; }

        [Column("error_type")]
        public ErrorTypes ErrorType { get; set; }

        [Column("collection_time")]
        public DateTime CollectionTime { get; set; }
    }
}