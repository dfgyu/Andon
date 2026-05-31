namespace Andon.Dtos
{
    public class IoTEquipmentRTDReportDto
    {
        public int EquipmentId { get; set; }
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
        public decimal? Value3 { get; set; }
        public decimal? Value4 { get; set; }
        public decimal? Value5 { get; set; }
        public decimal? Value6 { get; set; }

        public string? Label1 { get; set; }
        public string? Label2 { get; set; }
        public string? Label3 { get; set; }
        public string? Label4 { get; set; }
        public string? Label5 { get; set; }
        public string? Label6 { get; set; }
    }
}
