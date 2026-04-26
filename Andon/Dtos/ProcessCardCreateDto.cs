using System.ComponentModel.DataAnnotations;    

namespace Andon.Dtos
{
    public class ProcessCardCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string? CardCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ProcessName { get; set; }

        public int? EquipmentId { get; set; }
        public int? OperatorId { get; set; }
        public int? EstTime { get; set; }
    }
}
