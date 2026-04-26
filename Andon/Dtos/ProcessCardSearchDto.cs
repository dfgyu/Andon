namespace Andon.Dtos
{
    public class ProcessCardSearchDto
    {
        public string? CardCode { get; set; }
        public string? ProcessName { get; set; }
        public int? EquipmentId { get; set; }
        public int? OperatorId { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
