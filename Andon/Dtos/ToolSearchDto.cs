namespace Andon.Dtos
{
    public class ToolSearchDto
    {
        public string? ToolName { get; set; }
        public string? ToolModel { get; set; }
        public string? Warehouse { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}