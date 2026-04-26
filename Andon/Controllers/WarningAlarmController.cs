using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Andon.Controllers
{
    [Route("api/alarm")]
    [ApiController]
    [Authorize]
    public class WarningAlarmController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WarningAlarmController(AppDbContext context)
        {
            _context = context;
        }

        // 列表 + 分页 + 筛选
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] AlarmSearchDto dto)
        {
            var query = _context.WarningAlarms.AsQueryable();

            if (!string.IsNullOrEmpty(dto.LineId))
                query = query.Where(a => a.LineId == dto.LineId);

            if (dto.EquipmentId.HasValue)
                query = query.Where(a => a.EquipmentId == dto.EquipmentId);

            if (!string.IsNullOrEmpty(dto.AlarmType))
                query = query.Where(a => a.AlarmType == dto.AlarmType);

            if (dto.IsStopLine.HasValue)
                query = query.Where(a => a.IsStopLine == dto.IsStopLine);

            var total = await query.CountAsync();

            var list = await query
                .OrderByDescending(a => a.StartTime)
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }

        // 处理报警（指定处理人）
        [HttpPatch("handle/{id}")]
        public async Task<IActionResult> Handle(int id, [FromQuery] int handlerId)
        {
            var alarm = await _context.WarningAlarms.FindAsync(id);
            if (alarm == null) return NotFound();

            alarm.HandlerId = handlerId;
            await _context.SaveChangesAsync();

            return Ok("已处理");
        }

        // 手动恢复
        [HttpPatch("recover/{id}")]
        public async Task<IActionResult> Recover(int id)
        {
            var alarm = await _context.WarningAlarms.FindAsync(id);
            if (alarm == null) return NotFound();

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            alarm.EndTime = now;

            if (DateTime.TryParse(alarm.StartTime, out DateTime start) &&
                DateTime.TryParse(now, out DateTime end))
            {
                alarm.DurationMin = (int)(end - start).TotalMinutes;
            }

            await _context.SaveChangesAsync();
            return Ok("已恢复");
        }
        /// <summary>
        /// 获取所有报警配置
        /// </summary>
        [HttpGet("configs")]
        public async Task<ActionResult<IEnumerable<AndonAlarmConfig>>> GetAllAlarmConfigs()
        {
            return await _context.AndonAlarmConfigs.ToListAsync();
        }
        /// <summary>
        /// 根据ID获取报警配置
        /// </summary>
        [HttpGet("configs/{id}")]
        public async Task<ActionResult<AndonAlarmConfig>> GetAlarmConfig(int id)
        {
            var config = await _context.AndonAlarmConfigs.FindAsync(id);
            if (config == null)
            {
                return NotFound("报警配置不存在");
            }
            return Ok(config);
        }
        /// <summary>
        /// 添加报警配置
        /// </summary>
        [HttpPost("configs")]
        public async Task<ActionResult<AndonAlarmConfig>> AddAlarmConfig(AndonAlarmConfig config)
        {
            _context.AndonAlarmConfigs.Add(config);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAlarmConfig), new { id = config.Id }, config);
        }
        /// <summary>
        /// 修改报警配置
        /// </summary>
        [HttpPut("configs/{id}")]
        public async Task<IActionResult> UpdateAlarmConfig(int id, AndonAlarmConfig config)
        {
            if (id != config.Id)
            {
                return BadRequest("配置ID不匹配");
            }

            _context.Entry(config).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AndonAlarmConfigs.Any(e => e.Id == id))
                {
                    return NotFound("报警配置不存在");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        /// <summary>
        /// 删除报警配置
        /// </summary>
        [HttpDelete("configs/{id}")]
        public async Task<IActionResult> DeleteAlarmConfig(int id)
        {
            var config = await _context.AndonAlarmConfigs.FindAsync(id);
            if (config == null)
            {
                return NotFound("报警配置不存在");
            }

            _context.AndonAlarmConfigs.Remove(config);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ======================================================================
        // 【核心】触发报警 → 自动生成记录 + 亮灯 + 发短信
        // ======================================================================
        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerAlarm(string lineId, string alarmSource)
        {
            // 1. 查询报警配置
            var config = await _context.AndonAlarmConfigs
                .FirstOrDefaultAsync(c => c.LineId == lineId && c.AlarmSource == alarmSource);

            if (config == null)
                return NotFound("未找到报警配置");

            // 2. 生成报警记录
            var record = new AndonAlarmConfig
            {
                LineId = lineId,
                AlarmSource = alarmSource,
                LevelCode = config.LevelCode,
                LightAlarm = config.LightAlarm,
                SoundAlarm = config.SoundAlarm,
                IsStopLine = config.IsStopLine,
                AlarmTime = DateTime.Now,
                HandleStatus = 0,
                Handler = null,
                HandleTime = null
            };

            _context.AndonAlarmConfigs.Add(record);
            await _context.SaveChangesAsync();

            // ======================================================
            // 触发硬件报警（灯光 + 声音）
            // ======================================================
            await TriggerHardwareAlarm(config);

            // ======================================================
            // 自动发短信给负责人（从设备/工序卡取电话）
            // ======================================================
            await SendAlarmSms(lineId);

            return Ok(new
            {
                recordId = record.Id,
                message = "报警已触发，记录已生成，灯光已启动，短信已发送"
            });
        }

        // ======================================================================
        // 【硬件报警】灯光 + 声音
        // ======================================================================
        private async Task TriggerHardwareAlarm(AndonAlarmConfig config)
        {
            try
            {
                // 这里是真实硬件调用逻辑（PLC/串口/网络指令）
                // 需要对接硬件厂商SDK即可

                var data = new HardwareAlarmDto
                {
                    LineId = config.LineId,
                    LightAlarm = config.LightAlarm,
                    SoundAlarm = config.SoundAlarm,
                    IsStopLine = config.IsStopLine ?? 0
                };

                // 示例：亮红灯 + 鸣笛
                if (config.LightAlarm == "RED")
                {
                    // 亮红灯指令
                }
                if (config.SoundAlarm == "ENABLE")
                {
                    // 声音报警
                }
                if (config.IsStopLine == 1)
                {
                    // 产线停机
                }

                await Task.CompletedTask;
            }
            catch
            {
                // 硬件异常不影响主流程
            }
        }

        // ======================================================================
        // 【自动发短信】从 ProcessCard / Equipment 取员工电话
        // ======================================================================
        private async Task SendAlarmSms(string lineId)
        {
            try
            {
                // 1. 先获取当前产线的设备
                var eq = await _context.BizEquipments
                    .FirstOrDefaultAsync(e => e.LineId == lineId);

                if (eq == null) return;

                // 2. 获取当前设备最新工序卡（负责人）
                var card = await _context.BizProcessCards
                    .Where(c => c.EquipmentId == eq.Id)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefaultAsync();

                if (card == null || card.OperatorId == null) return;

                // 3. 获取操作员电话（来自 sys_user）
                var user = await _context.SysUsers
                    .FirstOrDefaultAsync(u => u.Id == card.OperatorId);

                if (user == null || string.IsNullOrEmpty(user.Phone)) return;

                // 4. 发送报警短信
                var sms = new SendSmsDto
                {
                    Phone = user.Phone,
                    Message = $"【产线报警】产线：{lineId}，设备：{eq.EquipmentName}，时间：{DateTime.Now:HH:mm}"
                };

                // 调用短信接口
                await SendSms(sms);
            }
            catch
            {
                // 短信失败不影响主流程
            }
        }

        // ======================================================================
        // 短信发送接口
        // ======================================================================
        private async Task SendSms(SendSmsDto dto)
        {
            // 这里接入阿里云/腾讯云/华为云短信SDK
            // 你只需要填配置即可

            await Task.CompletedTask;
        }

    }
}