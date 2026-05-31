using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Andon.Models
{
    [Table("biz_equipment_realtime_data")]
    public class BizEquipmentRTD
    {
        [Key]
        public int Id { get; set; }

        [Column("equipment_name")]
        public string? EquipmentName { get; set; }

        [Column("equipment_id")]
        public int EquipmentId { get; set; }

        [Column("equipment_code")]
        public string? EquipmentCode { get; set; }

        [Column("value1")]
        public decimal? Value1 { get; set; }

        [Column("value2")]
        public decimal? Value2 { get; set; }

        [Column("value3")]
        public decimal? Value3 { get; set; }

        [Column("value4")]
        public decimal? Value4 { get; set; }

        [Column("value5")]
        public decimal? Value5 { get; set; }

        [Column("value6")]
        public decimal? Value6 { get; set; }

        [Column("label1")]
        public string? Label1 { get; set; }

        [Column("label2")]
        public string? Label2 { get; set; }

        [Column("label3")]
        public string? Label3 { get; set; }

        [Column("label4")]
        public string? Label4 { get; set; }

        [Column("label5")]
        public string? Label5 { get; set; }

        [Column("label6")]
        public string? Label6 { get; set; }

        [Column("create_at")]
        public DateTime? CreateAt { get; set; } = DateTime.Now;

    }
}
