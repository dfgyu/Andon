using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Andon.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


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

        [Column("equipment_model")]
        [MaxLength(50)]
        public string? EquipmentModel { get; set; }


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

        [Column("installation_date")]
        [MaxLength(100)]
        public DateTime? InstallationDate { get; set; }

        [Column("maintenance_date")]
        [MaxLength(100)]
        public DateTime? MaintenanceDate { get; set; }
    }
}
