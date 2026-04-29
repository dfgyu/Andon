using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    public class ToolCreateDto
    {
        [MaxLength(50)]
        public string? ToolName { get; set; }

        [MaxLength(50)]
        public string? ToolModel { get; set; }

        public int? TotalQty { get; set; }

        public int SurplusQty { get; set; } = 0;

        [MaxLength(20)]
        public string? Warehouse { get; set; }
    }
}