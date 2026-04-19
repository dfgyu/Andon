using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Andon.Models
{
    [Table("biz_quality_inspection")]
    public class BizQualityInspection
    {
        [Key]
        public int Id { get; set; }

        [Column("product_name")]
        [MaxLength(100)]
        public string? ProductName { get; set; }

        [Column("operator_id")]
        public int? OperatorId { get; set; }

        [Column("total_qty")]
        public int? TotalQty { get; set; }

        [Column("qualified_qty")]
        public int? QualifiedQty { get; set; }

        [Column("unqualified_qty")]
        public int? UnqualifiedQty { get; set; }

        [Column("is_qualified")]
        public int? IsQualified { get; set; } // 0不合格 1合格
    }
}
