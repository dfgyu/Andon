using Andon.Dtos;
using Andon.Enums;
using Andon.Hubs;
using Andon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;



namespace Andon.Controllers
{
    [Route("api/iot")]
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<LineMonitorHub> _hubContext;  
        public IotController(AppDbContext context, IHubContext<LineMonitorHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // ==========================================
        // 【IoT 设备实时上报】→ 写入状态 + 自动报警
        // ==========================================
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] IotUploadDto dto)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 1. 写入实时状态表
            var data = new BizIotEquipmentData
            {
                LineId = dto.LineId,
                RunStatus = dto.RunStatus,
                IsBlocked = dto.IsBlocked,
                IsOverheat = dto.IsOverheat,
                IsDeviation = dto.IsDeviation,
                IsPackError = dto.IsPackError,
                CollectionTime = now
            };

            _context.BizIotEquipmentDatas.Add(data);
            await _context.SaveChangesAsync();

            // 2. 获取设备信息
            var eq = await _context.BizEquipments
                .FirstOrDefaultAsync(e => e.LineId == dto.LineId);

            // 3. 判断是否触发报警
            bool needAlarm =
                dto.RunStatus == (int)IotRunStatus.故障 ||
                dto.RunStatus == (int)IotRunStatus.报警中 ||
                dto.IsBlocked  ||
                dto.IsOverheat  ||
                dto.IsDeviation  ||
                dto.IsPackError ;

            if (needAlarm)
                await CreateAlarm(dto, now);

            // 4. 自动恢复报警
            else
                await AutoRecover(dto.LineId, now);


            // ======================
            // 【SignalR 实时推送】
            // ======================
            var latestData = new
            {
                equipmentId = eq?.Id,
                lineId = dto.LineId,
                runStatus = dto.RunStatus,
                isBlocked = dto.IsBlocked,
                isOverheat = dto.IsOverheat,
                isDeviation = dto.IsDeviation,
                isPackError = dto.IsPackError,
                collectionTime = now,
                isAlarm = needAlarm
            };

            // 推送给所有监听该产线的前端
            await _hubContext.Clients.Group(dto.LineId).SendAsync("ReceiveEquipmentStatus", latestData);

            return Ok(new
            {
                success = true,
                alarm = needAlarm,
                dataId = data.Id
            });

            
        }

        // ==========================================
        // 创建报警（完全按你的 warning_alarms）
        // ==========================================
        private async Task CreateAlarm(IotUploadDto dto, string now)
        {
            // 同一产线已存在报警 → 不重复创建
            bool exists = await _context.WarningAlarms
                .AnyAsync(a => a.LineId == dto.LineId && a.EndTime == null);

            if (exists) return;

            // 获取设备信息
            var eq = await _context.BizEquipments
                .FirstOrDefaultAsync(e => e.LineId == dto.LineId);

            int? equipmentId = eq?.Id;
            var process = eq?.Process ?? EquipmentsProcess.其他;

            // 报警内容
            var (type, desc) = GetAlarmInfo(dto);

            var alarm = new WarningAlarms
            {
                LineId = dto.LineId,
                EquipmentId = equipmentId,
                Process = process,
                AlarmConfigId = null,
                AlarmDesc = desc,
                StartTime = now,
                EndTime = null,
                DurationMin = null,
                AlarmType = type,
                IsStopLine = dto.RunStatus == 2 ? 1 : 0,
                HandlerId = null
            };

            _context.WarningAlarms.Add(alarm);
            await _context.SaveChangesAsync();

           
        }

        // ==========================================
        // 自动恢复报警 + 计算时长
        // ==========================================
        private async Task AutoRecover(string lineId, string now)
        {
            var alarms = await _context.WarningAlarms
                .Where(a => a.LineId == lineId && a.EndTime == null)
                .ToListAsync();

            foreach (var alarm in alarms)
            {
                alarm.EndTime = now;

                if (DateTime.TryParse(alarm.StartTime, out DateTime start) &&
                    DateTime.TryParse(now, out DateTime end))
                {
                    alarm.DurationMin = (int)(end - start).TotalMinutes;
                }
            }

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // 报警类型匹配
        // ==========================================
        private (string type, string desc) GetAlarmInfo(IotUploadDto dto)
        {
            if (dto.IsBlocked ) return ("堵料", "设备堵料");
            if (dto.IsOverheat ) return ("过热", "电机过热/过载");
            if (dto.IsDeviation ) return ("跑偏", "皮带跑偏");
            if (dto.IsPackError ) return ("包装", "包装卡袋/漏封");
            if (dto.RunStatus == 2) return ("故障", "设备故障");
            if (dto.RunStatus == 3) return ("报警", "设备报警中");
            return ("未知", "设备异常");
        }
    }
}