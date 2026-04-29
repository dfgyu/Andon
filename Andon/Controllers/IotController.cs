using Andon.Dtos;
using Andon.Hubs;
using Andon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace Andon.Controllers
{
    /// <summary>
    /// ioT设备数据上报接口
    /// </summary>
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

        // ================================
        // IoT设备上报（自动匹配报警配置）
        // ================================

        /// <summary>
        ///
        ///  设备上报接口，接收IoT设备发送的实时数据，根据产线+报警类型自动匹配报警配置，生成报警记录，并推送到前端大屏
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] IotUploadDto dto)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 1. 根据EquipmentId精准定位设备
            var equipment = await _context.BizEquipments
                .FirstOrDefaultAsync(e => e.Id == dto.EquipmentId);

            if (equipment == null)
                return NotFound("设备不存在");

            // 2. 保存IoT实时数据
            var iotData = new BizIotEquipmentData
            {
                LineId = dto.LineId,
                EquipmentId = dto.EquipmentId,
                RunStatus = dto.RunStatus,
                IsBlocked = dto.IsBlocked,
                IsOverheat = dto.IsOverheat,
                IsDeviation = dto.IsDeviation,
                IsPackError = dto.IsPackError,
                CollectionTime = now
            };

            _context.BizIotEquipmentDatas.Add(iotData);
            await _context.SaveChangesAsync();

            // 3. 判断是否产生报警
            bool isAlarm = dto.RunStatus == 2 || dto.RunStatus == 3
                        || dto.IsBlocked 
                        || dto.IsOverheat 
                        || dto.IsDeviation 
                        || dto.IsPackError;

            if (isAlarm)
            {
                // 获取报警类型
                string alarmType = GetAlarmType(dto);

                // ==============================================
                // 根据产线+报警类型 查询报警配置
                // ==============================================
                var alarmConfig = await _context.AndonAlarmConfigs
                    .FirstOrDefaultAsync(c => c.LineId == equipment.LineId
                                          && c.AlarmSource == alarmType);

                // 创建报警记录（带配置信息）
                await CreateAlarmRecord(equipment, dto, alarmConfig, now);

                // 触发硬件（灯、声音、停机）
                await TriggerHardwareAlarm(alarmConfig);

                // 发送短信给操作员
                await SendAlarmSms(equipment);
            }
            else
            {
                // 无异常 → 自动恢复报警
                await AutoRecoverEquipmentAlarm(dto.EquipmentId);
            }

            // 4. 推送到前端大屏（SignalR）
            await PushStatusToDashboard(equipment, dto, isAlarm, now);

            return Ok(new
            {
                success = true,
                equipmentId = equipment.Id,
                equipmentName = equipment.EquipmentName,
                isAlarm
            });
        }

        // ==============================================
        // 创建报警记录（关联报警配置）
        // ==============================================

        /// <summary>
        /// 创建报警记录，关联产线+报警类型的报警配置，避免重复报警
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="dto"></param>
        /// <param name="config"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        private async Task CreateAlarmRecord(BizEquipment equipment, IotUploadDto dto, AndonAlarmConfig? config, string now)
        {
            bool exists = await _context.WarningAlarms
                .AnyAsync(a => a.EquipmentId == equipment.Id && a.EndTime == null);

            if (exists) return;

            var alarm = new WarningAlarms
            {
                LineId = dto.LineId,
                EquipmentId = equipment.Id,
                Process = equipment.Process,
                AlarmType = GetAlarmType(dto),
                AlarmDesc = GetAlarmDesc(dto),
                StartTime = now,
                EndTime = null,
                DurationMin = null,
                IsStopLine = config?.IsStopLine ?? 0,
                HandlerId = null
            };

            _context.WarningAlarms.Add(alarm);
            await _context.SaveChangesAsync();
        }

        // ==============================================
        // 触发硬件报警（灯光、声音、停机）
        // ==============================================
        private async Task TriggerHardwareAlarm(AndonAlarmConfig? config)
        {
            if (config == null) return;

            try
            {
                // 亮灯
                if (config.LightAlarm == "RED")
                {
                    // 红灯指令
                }
                else if (config.LightAlarm == "YELLOW")
                {
                    // 黄灯指令
                }

                // 声音
                if (config.SoundAlarm == "ENABLE")
                {
                    // 鸣笛
                }

                // 停机
                if (config.IsStopLine == 1)
                {
                    // 停机指令
                }

                await Task.CompletedTask;
            }
            catch
            {
                // 硬件异常不影响系统
            }
        }

        // ==============================================
        // 发送报警短信给当前设备操作员
        // ==============================================
        private async Task SendAlarmSms(BizEquipment equipment)
        {
            try
            {
                var card = await _context.BizProcessCards
                    .Where(c => c.EquipmentId == equipment.Id)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefaultAsync();

                if (card?.OperatorId == null) return;

                var user = await _context.SysUsers.FindAsync(card.OperatorId);
                if (user == null || string.IsNullOrEmpty(user.Phone)) return;

                // 短信发送逻辑（接入阿里云/腾讯云）
                await Task.CompletedTask;
            }
            catch
            {
                // 短信失败不影响主流程
            }
        }

        // ==============================================
        // 自动恢复报警
        // ==============================================
        private async Task AutoRecoverEquipmentAlarm(int equipmentId)
        {
            var alarms = await _context.WarningAlarms
                .Where(a => a.EquipmentId == equipmentId && a.EndTime == null)
                .ToListAsync();

            foreach (var alarm in alarms)
            {
                alarm.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (DateTime.TryParse(alarm.StartTime, out DateTime start) &&
                    DateTime.TryParse(alarm.EndTime, out DateTime end))
                {
                    alarm.DurationMin = (int)(end - start).TotalMinutes;
                }
            }

            await _context.SaveChangesAsync();
        }

        // ==============================================
        // 推送到大屏
        // ==============================================
        private async Task PushStatusToDashboard(BizEquipment eq, IotUploadDto dto, bool isAlarm, string now)
        {
            var data = new
            {
                equipmentId = eq.Id,
                lineId = dto.LineId,
                equipmentName = eq.EquipmentName,
                runStatus = dto.RunStatus,
                isAlarm,
                alarmMsg = isAlarm ? GetAlarmDesc(dto) : "",
                time = now
            };

            await _hubContext.Clients.Group(dto.LineId).SendAsync("ReceiveEquipmentStatus", data);
        }

        // ==============================================
        // 报警类型
        // ==============================================
        private string GetAlarmType(IotUploadDto dto)
        {
            if (dto.IsBlocked) return "堵料";
            if (dto.IsOverheat) return "过热";
            if (dto.IsDeviation) return "跑偏";
            if (dto.IsPackError) return "包装异常";
            if (dto.RunStatus == 2) return "故障";
            return "未知";
        }

        private string GetAlarmDesc(IotUploadDto dto)
        {
            if (dto.IsBlocked ) return "设备堵料";
            if (dto.IsOverheat) return "电机过热";
            if (dto.IsDeviation) return "皮带跑偏";
            if (dto.IsPackError) return "包装卡袋/漏封";
            if (dto.RunStatus == 2) return "设备故障停机";
            return "设备异常";
        }
    }
}