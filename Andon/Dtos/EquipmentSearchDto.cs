using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Andon.Enums;


namespace Andon.Dtos
{
    public class EquipmentSearchDto
    {
        public string? EquipmentCode { get; set; }
        public string? EquipmentName { get; set; }
        public string? LineId { get; set; }
        public EquipmentStatus? Status { get; set; } // 可空枚举筛选

        public EquipmentsProcess? Process { get; set; }
        public string? AlertContact { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
