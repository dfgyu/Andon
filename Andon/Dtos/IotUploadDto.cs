using Andon.Enums;
using System.ComponentModel.DataAnnotations;

namespace Andon.Dtos
{
    public class IotUploadDto
    {
        [Required]
        public string LineId { get; set; } = "";

        [Required] // 必须传设备ID
        public int EquipmentId { get; set; }

        [Required]
        public EquipmentStatus RunStatus { get; set; }
        
        public ErrorTypes ErrorType { get; set; }
    }
}