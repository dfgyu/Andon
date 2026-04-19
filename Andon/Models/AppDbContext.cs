using Microsoft.EntityFrameworkCore;


namespace Andon.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<AndonAlarmConfig> AndonAlarmConfigs { get; set; }

        public DbSet<ProductionEquipmentData> ProductionEquipmentData { get; set; }

        public DbSet<WarningAlarms> WarningAlarms { get; set; }

        public DbSet<SysRole> SysRoles { get; set; }

        public DbSet<BizAttendance> BizAttendances { get; set; }

        public DbSet<BizEquipment> BizEquipments { get; set; }

        public DbSet<BizMaterial> BizMaterials { get; set; }

        public DbSet<BizProcessCard> BizProcessCards { get; set; }

        public DbSet<BizQualityInspection> BizQualityInspections { get; set; }

        public DbSet<BizTool> BizTools { get; set; }

    }
}
