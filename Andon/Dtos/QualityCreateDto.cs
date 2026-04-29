using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    public class QualityCreateDto
    {
        [MaxLength(100)]
        public string? ProductName { get; set; }

        public int? OperatorId { get; set; }

        public int? TotalQty { get; set; }

        public int? QualifiedQty { get; set; }

        public int? UnqualifiedQty { get; set; }

        public int? IsQualified { get; set; } // 0不合格 1合格
    }
}