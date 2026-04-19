using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Andon.Models
{
    [Table("biz_material")]
    public class BizMaterial
    {
        [Key]
        public int Id { get; set; }

        [Column("material_code")]
        [MaxLength(50)]
        public string? MaterialCode { get; set; }

        [Column("material_name")]
        [MaxLength(100)]
        public string? MaterialName { get; set; }

        [Column("size")]
        [MaxLength(50)]
        public string? Size { get; set; }

        [Column("weight")]
        [MaxLength(20)]
        public string? Weight { get; set; }

        [Column("type")]
        [MaxLength(20)]
        public string? Type { get; set; }

        [Column("surplus_qty")]
        public int SurplusQty { get; set; } = 0;

    }
}
