using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Andon.Models
{
    [Table("biz_tool")]
    public class BizTool
    {
        [Key]
        public int Id { get; set; }

        [Column("tool_name")]
        [MaxLength(50)]
        public string? ToolName { get; set; }

        [Column("tool_model")]
        [MaxLength(50)]
        public string? ToolModel { get; set; }

        [Column("total_qty")]
        public int? TotalQty { get; set; }

        [Column("surplus_qty")]
        public int SurplusQty { get; set; } = 0;

        [Column("warehouse")]
        [MaxLength(20)]
        public string? Warehouse { get; set; }
    }
}
