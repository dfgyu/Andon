using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Andon.Models
{
    [Table("ProductionEquipmentData")]
    public class ProductionEquipmentData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("equipment_id")]
        public int EquipmentId { get; set; }

        [Column("temperature")]
        [MaxLength(20)]
        public string? Temperature { get; set; }

        [Column("humidity")]
        [MaxLength(20)]
        public string? Humidity { get; set; }

        [Column("vibration")]
        [MaxLength(20)]
        public string? Vibration { get; set; }

        [Column("collection_time")]
        [MaxLength(30)]
        public string? CollectionTime { get; set; }
    }
}
