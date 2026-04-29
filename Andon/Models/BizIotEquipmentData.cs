using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Andon.Models
{
    [Table("ProductionEquipmentData")]
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
        public int RunStatus { get; set; }

        [Column("is_blocked")]
        public bool IsBlocked { get; set; }

        [Column("is_overheat")]
        public bool IsOverheat { get; set; }

        [Column("is_deviation")]
        public bool IsDeviation { get; set; }

        [Column("is_pack_error")]
        public bool IsPackError { get; set; }

        [Column("collection_time")]
        [MaxLength(50)]
        public string CollectionTime { get; set; } = "";
    }
}