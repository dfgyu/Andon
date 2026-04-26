using System.ComponentModel.DataAnnotations;
using Andon.Enums;

namespace Andon.Dtos
{
    public class IotEquipmentDataCreateDto
    {
        [Required]
        public string LineId { get; set; } = "";

        [Required]
        public int RunStatus { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsOverheat { get; set; }
        public bool IsDeviation { get; set; }
        public bool IsPackError { get; set; }
    }
}
