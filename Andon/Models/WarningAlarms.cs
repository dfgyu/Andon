using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Andon.Models
{
    [Table("warning_alarms")]
    public class WarningAlarms
    {
        [Key]
        public int Id { get; set; }

        [Column("line_id")]
        [MaxLength(20)]
        public string? LineId { get; set; }

        [Column("equipment_id")]
        public int? EquipmentId { get; set; }

        [Column("station_code")]
        [MaxLength(20)]
        public string? StationCode { get; set; }

        [Column("alarm_desc")]
        [MaxLength(255)]
        public string? AlarmDesc { get; set; }

        [Column("start_time")]
        [MaxLength(30)]
        public string? StartTime { get; set; }

        [Column("end_time")]
        [MaxLength(30)]
        public string? EndTime { get; set; }

        [Column("duration_min")]
        public int? DurationMin { get; set; }

        [Column("alarm_type")]
        [MaxLength(20)]
        public string? AlarmType { get; set; }

        [Column("is_stop_line")]
        public int? IsStopLine { get; set; }

        [Column("handler_id")]
        public int? HandlerId { get; set; }
    }
}
