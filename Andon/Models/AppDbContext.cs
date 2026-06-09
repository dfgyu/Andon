using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Andon.Enums;


namespace Andon.Models
{
    public class AppDbContext : DbContext
    {
        private readonly ILogger<AppDbContext>? _logger;

        public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext>? logger = null) : base(options)
        {
            _logger = logger;
        }

        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<AndonAlarmConfig> AndonAlarmConfigs { get; set; }

        public DbSet<BizIotEquipmentData> BizIotEquipmentDatas { get; set; }

        public DbSet<WarningAlarms> WarningAlarms { get; set; }

        public DbSet<SysRole> SysRoles { get; set; }

        public DbSet<BizAttendance> BizAttendances { get; set; }

        public DbSet<BizEquipment> BizEquipments { get; set; }

        public DbSet<BizMaterial> BizMaterials { get; set; }


        public DbSet<BizQualityInspection> BizQualityInspections { get; set; }

        public DbSet<BizTool> BizTools { get; set; }

        public DbSet<BizEquipmentRTD> BizEquipmentRTDs { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entries = ChangeTracker.Entries<BizEquipment>()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

                foreach (var entry in entries)
                {
                    var prop = entry.Property(e => e.Status);
                    if (prop != null)
                    {
                        var original = prop.OriginalValue;
                        var current = prop.CurrentValue;
                        if (!EqualityComparer<EquipmentStatus>.Default.Equals(original, current))
                        {
                            var stack = new StackTrace(2, true).ToString();
                            _logger?.LogWarning("设备状态状态 Id={Id} Old={Old} New={New}\nStack:\n{Stack}",
                                entry.Entity.Id, (int)original, (int)current, stack);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 不抛异常，记录日志用于排查
                _logger?.LogError(ex, "Error while scanning BizEquipment changes");
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
