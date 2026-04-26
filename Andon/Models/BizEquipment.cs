using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Andon.Enums;


namespace Andon.Models
{
    [Table("biz_equipment")]
    public class BizEquipment
    {
        [Key]
        public int Id { get; set; }

        [Column("equipment_code")]
        [MaxLength(50)]
        public string? EquipmentCode { get; set; }

        [Column("equipment_name")]
        [MaxLength(100)]
        public string? EquipmentName { get; set; }

        [Column("process")]
        public EquipmentsProcess Process { get; set; }

        [Column("line_id")]
        [MaxLength(20)]
        public string? LineId { get; set; }

        [Column("status")]
        public EquipmentStatus Status { get; set; }

        [Column("alert_contact")]
        [MaxLength(100)]
        public string? AlertContact { get; set; }
    }
}
