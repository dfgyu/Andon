namespace Andon.Dtos
{
    public class QualitySearchDto
    {
        public string? ProductName { get; set; }
        public int? OperatorId { get; set; }
        public int? IsQualified { get; set; } // 按合格/不合格筛选

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}