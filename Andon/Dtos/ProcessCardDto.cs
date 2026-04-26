namespace Andon.Dtos
{
    public class ProcessCardDto
    {
        public int Id { get; set; }
        public string? CardCode { get; set; }
        public string? ProcessName { get; set; }

        public int? EquipmentId { get; set; }
        public string? EquipmentName { get; set; } // 设备名称（关联查询）

        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; } // 操作员名称（关联查询）

        public int? EstTime { get; set; }
    }
}
