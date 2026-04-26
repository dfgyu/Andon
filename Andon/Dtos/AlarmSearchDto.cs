namespace Andon.Dtos
{
    public class AlarmSearchDto
    {
        public string? LineId { get; set; }
        public int? EquipmentId { get; set; }
        public string? AlarmType { get; set; }
        public int? IsStopLine { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}