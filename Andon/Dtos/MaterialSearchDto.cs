namespace Andon.Dtos
{
    public class MaterialSearchDto
    {
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? Type { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
