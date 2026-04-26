using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Andon.Models
{
    [Table("andon_alarm_config")]
    public class AndonAlarmConfig
    {
        [Key]
        public int Id { get; set; }

        [Column("line_id")]
        [MaxLength(20)]
        public string? LineId { get; set; }

        [Column("alarm_source")]
        [MaxLength(50)]
        public string? AlarmSource { get; set; }

        [Column("level_code")]
        [MaxLength(10)]
        public string? LevelCode { get; set; }

        [Column("light_alarm")]
        [MaxLength(20)]
        public string? LightAlarm { get; set; }

        [Column("sound_alarm")]
        [MaxLength(100)]
        public string? SoundAlarm { get; set; }


        [Column("is_stop_line")]
        public int? IsStopLine { get; set; }

        [Column("alarm_time")]
        public DateTime AlarmTime { get; set; }

        [Column("handle_status")]
        public int HandleStatus { get; set; } = 0;

        [Column("handler")]
        [MaxLength(50)]
        public string? Handler { get; set; }

        [Column("handle_time")]
        public DateTime? HandleTime { get; set; }
    }
}
