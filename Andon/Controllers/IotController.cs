using Andon.Dtos;
using Andon.Helpers;
using Andon.Hubs;
using Andon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Andon.Controllers
{
    [Route("api/iot")]
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<EquipmentRTDHub> _hubContext;
        private readonly ILogger<IotController> _logger;

        public IotController(AppDbContext context, IHubContext<EquipmentRTDHub> hubContext, ILogger<IotController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost("EquipmentRTDUpload")]
        public async Task<IActionResult> EquipmentRTDUpload([FromBody] IoTEquipmentRTDReportDto dto)
        {
            DateTime now = DateTime.Now;
            var equipment = await _context.BizEquipments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == dto.EquipmentId);
            if (equipment == null)
                return NotFound("设备不存在");
            var EquipmentRTD = new BizEquipmentRTD
            {
                EquipmentId = dto.EquipmentId,
                EquipmentCode = equipment.EquipmentCode,
                EquipmentName = equipment.EquipmentName,
                Value1 = dto.Value1,
                Value2 = dto.Value2,
                Value3 = dto.Value3,
                Value4 = dto.Value4,
                Value5 = dto.Value5,
                Value6 = dto.Value6,
                Label1 = dto.Label1,
                Label2 = dto.Label2,
                Label3 = dto.Label3,
                Label4 = dto.Label4,
                Label5 = dto.Label5,
                Label6 = dto.Label6,
                CreateAt = now
            };

            _context.BizEquipmentRTDs.Add(EquipmentRTD);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(dto.EquipmentId.ToString())
               .SendAsync("ReceiveRealData", EquipmentRTD);

            
            return Ok(new
            {
                success = true,
                equipmentId = equipment.Id,
                equipmentName = equipment.EquipmentName
            });
        }

        [HttpPost("ErrorUpload")]
        public async Task<IActionResult> ErrorUpload([FromBody] IotUploadDto dto)
        {
            DateTime now = DateTime.Now;


            var equipment = await _context.BizEquipments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == dto.EquipmentId);

            if (equipment == null)
                return NotFound("设备不存在");

            var iotData = new BizIotEquipmentData
            {
                LineId = dto.LineId,
                EquipmentId = dto.EquipmentId,
                RunStatus = dto.RunStatus, 
                ErrorType = dto.ErrorType,
                CollectionTime = now
            };

            _context.BizIotEquipmentDatas.Add(iotData);
            await _context.SaveChangesAsync();

            bool isAlarm = AlarmHelper.IsAlarm(dto.RunStatus, dto.ErrorType);

            if (isAlarm)
            {
                var alarmType = dto.ErrorType.ToString();
                var alarmConfig = await _context.AndonAlarmConfigs
                    .FirstOrDefaultAsync(c => c.LineId == equipment.LineId && c.AlarmSource == alarmType);

                if (alarmConfig == null) {

                    return NotFound("报警配置不存在");
                }

                await CreateAlarmRecord(equipment, dto, alarmConfig, now);
                await TriggerHardwareAlarm(alarmConfig);
            }
            else
            {
                await AlarmHelper.AutoRecoverAlarm(_context, dto.EquipmentId);
            }

            await PushStatusToDashboard(equipment, dto, isAlarm, now);

            return Ok(new
            {
                success = true,
                equipmentId = equipment.Id,
                equipmentName = equipment.EquipmentName,
                isAlarm
            });
        }

        private async Task CreateAlarmRecord(BizEquipment equipment, IotUploadDto dto, AndonAlarmConfig? config, DateTime now)
        {
            bool exists = await _context.WarningAlarms
                .AnyAsync(a => a.EquipmentId == equipment.Id && a.EndTime == null);

            // 修正：获取实体对象并更新其状态
            var changeStatusEquipment = await _context.BizEquipments
                .FirstOrDefaultAsync(a => a.Id == dto.EquipmentId);
            if (changeStatusEquipment != null)
            {
                // 临时日志：写入前后记录状态，便于排查
                try
                {
                    var oldStatus = changeStatusEquipment.Status;
                    _logger.LogInformation("设备 {EquipmentId} 写入前状态：{OldStatus}，上报状态：{ReportedStatus}", changeStatusEquipment.Id, (int)oldStatus, (int)dto.RunStatus);
                }
                catch { }

                changeStatusEquipment.Status = dto.RunStatus;
                _context.BizEquipments.Update(changeStatusEquipment);
                await _context.SaveChangesAsync();

                try
                {
                    _logger.LogInformation("设备 {EquipmentId} 写入后状态：{NewStatus}", changeStatusEquipment.Id, (int)changeStatusEquipment.Status);
                }
                catch { }
            }

            if (exists) return;

            var alarm = new WarningAlarms
            {
                LineId = dto.LineId,
                EquipmentId = equipment.Id,
                Process = equipment.Process,
                AlarmType = dto.ErrorType.ToString(),
                AlarmDesc = AlarmHelper.GetAlarmDesc(dto.RunStatus, dto.ErrorType),
                StartTime = now,
                EndTime = null,
                DurationMin = null,
                AlarmConfigId = config.Id,
                IsStopLine = config?.IsStopLine ?? 0,
                HandlerId = null,
                Status = 0
            };

            _context.WarningAlarms.Add(alarm);
            await _context.SaveChangesAsync();
        }

        private async Task TriggerHardwareAlarm(AndonAlarmConfig? config)
        {
            if (config == null) return;
            try
            {
                await Task.CompletedTask;
            }
            catch { }
        }

        private async Task PushStatusToDashboard(BizEquipment eq, IotUploadDto dto, bool isAlarm, DateTime now)
        {
            var data = new
            {
                equipmentId = eq.Id,
                lineId = dto.LineId,
                equipmentName = eq.EquipmentName,
                runStatus = dto.RunStatus,
                isAlarm,
                alarmMsg = isAlarm ? AlarmHelper.GetAlarmDesc(dto.RunStatus, dto.ErrorType) : "",
                time = now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await _hubContext.Clients.Group(dto.LineId).SendAsync("ReceiveEquipmentStatus", data);
        }
    }
}