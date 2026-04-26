namespace Andon.Dtos
{
    public class HardwareAlarmDto
    {
        public string? LineId { get; set; }
        public string? LightAlarm { get; set; }
        public string? SoundAlarm { get; set; }
        public int IsStopLine { get; set; }
    }
}