using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    public class MaterialCreateDto
    {
        [MaxLength(50)]
        public string? MaterialCode { get; set; }

        [MaxLength(100)]
        public string? MaterialName { get; set; }

        [MaxLength(20)]
        public string? Type { get; set; }

        public int SurplusQty { get; set; } = 0;
    }
}