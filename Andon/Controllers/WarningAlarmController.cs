using Andon.Dtos;
using Andon.Helpers;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    /// <summary>
    /// 报警接口控制器：包含报警记录的查询、处理、恢复，以及报警配置的增删改查（CRUD）接口。
    /// 返回值统一使用 IActionResult 包装，常见返回结构在各方法的 <returns> 中做了详细说明。
    /// </summary>
    [Route("api/alarm")]
    [ApiController]
    public class WarningAlarmController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WarningAlarmController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allAlarms = await _context.WarningAlarms
                .AsNoTracking()
                .Join(
                    _context.BizEquipments,
                    alarm => alarm.EquipmentId,
                    equipment => equipment.Id,
                    (alarm, equipment) => new { alarm, equipment }
                )
                .Select(data => new
                {
                    LineId = data.alarm.LineId,
                    AlarmDesc = data.alarm.AlarmDesc,
                    IsStopLine = data.alarm.IsStopLine,
                    Status = data.alarm.Status,
                    EquipmentId = data.alarm.EquipmentId,
                    Id = data.alarm.Id,
                    AlarmConfigId = data.alarm.AlarmConfigId,
                    Process = data.alarm.Process,
                    AlarmType = data.alarm.AlarmType,
                    DurationMin = data.alarm.DurationMin,
                    StartTime = data.alarm.StartTime,
                    EndTime = data.alarm.EndTime,
                    EquipmentName = data.equipment.EquipmentName
                })
                .ToListAsync();

            return Ok(allAlarms);
        }



        // 报警列表
        /// <summary>
        /// 查询报警记录，支持按产线、设备、报警类型筛选，并分页返回总数和列表。
        /// </summary>
        /// <param name="dto">报警查询条件，见 <see cref="AlarmSearchDto"/>，包含分页参数 Page、Limit 及筛选条件 LineId、EquipmentId、AlarmType。</param>
        /// <returns>
        /// HTTP 200 OK 返回 JSON 对象：
        /// {
        ///   "total": int,                  // 符合条件的总记录数
        ///   "list": List&lt;WarningAlarm&gt; // 当前页的报警记录列表
        /// }
        /// 可能的其他返回：
        /// - 400 BadRequest：当传入参数无效（由模型绑定/验证触发）
        /// </returns>
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] AlarmSearchDto dto)
        {
            var query = _context.WarningAlarms.AsQueryable();

            if (!string.IsNullOrEmpty(dto.LineId))
                query = query.Where(a => a.LineId == dto.LineId);

            if (dto.EquipmentId.HasValue)
                query = query.Where(a => a.EquipmentId == dto.EquipmentId);

            if (!string.IsNullOrEmpty(dto.EquipmentName))
                query = query.Where(a => a.bizEquipment.EquipmentName == dto.EquipmentName);

            if (!string.IsNullOrEmpty(dto.LevelCode))
                query = query.Where(a => a.andonAlarmConfig.LevelCode == dto.LevelCode);

            if (dto.Status.HasValue)
                query = query.Where(a => a.Status == dto.Status);


            var total = await query.CountAsync();
            var list = await query
                .OrderByDescending(a => a.StartTime)
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }

        // 处理报警

        /// <summary>
        /// 处理报警接口，接收报警 ID 和处理人 ID，更新报警记录的处理人字段。
        /// </summary>
        /// <param name="id">报警记录的主键 ID。</param>
        /// <param name="handlerId">处理人 ID（用户标识）。</param>
        /// <returns>
        /// - HTTP 200 OK 返回字符串消息（类型：string）："已处理"
        /// - HTTP 404 NotFound：未找到对应的报警记录
        /// </returns>
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

        /// <summary>
        /// 手动恢复报警接口：为指定报警设置结束时间（默认为当前时间）并计算持续时长（分钟）。
        /// 用于无法由系统自动恢复的报警进行人工恢复。
        /// </summary>
        /// <param name="id">报警记录的主键 ID。</param>
        /// <returns>
        /// - HTTP 200 OK 返回字符串消息（type: string）："已恢复"
        /// - HTTP 404 NotFound：未找到对应的报警记录
        /// 返回后数据库中该报警记录的字段将被更新：
        /// - EndTime: string (格式 "yyyy-MM-dd HH:mm:ss")
        /// - DurationMin: int (持续分钟数，基于 StartTime 计算，如果解析失败则不修改)
        /// </returns>
        [HttpPost("recover/{equipmentId}")]
        public async Task<IActionResult> Recover(int equipmentId)
        {
            // 统一报警恢复入口
            await AlarmHelper.AutoRecoverAlarm(_context, equipmentId);
            return Ok("恢复成功");
        }
        // ====================== 报警配置 CRUD ======================
        /// <summary>
        /// 获取所有报警配置记录roleid = 6。
        /// </summary>
        /// <returns>
        /// HTTP 200 OK 返回 List&lt;AndonAlarmConfig&gt;，每项为一个报警配置对象。
        /// 结构示例：List&lt;AndonAlarmConfig&gt;，其中 AndonAlarmConfig 表示配置模型，包含 Id、LineId、AlarmType、Threshold 等字段（取决于模型定义）。
        /// </returns>
        [HttpGet("configs")]
        [Authorize]
        public async Task<IActionResult> GetConfigs()
        {
            var list = await _context.AndonAlarmConfigs.ToListAsync();
            return Ok(list);
        }
        /// <summary>
        /// 根据 ID 获取单个报警配置，用于编辑时加载数据。
        /// </summary>
        /// <param name="id">报警配置 ID。</param>
        /// <returns>
        /// - HTTP 200 OK 返回单个 <see cref="AndonAlarmConfig"/> 对象。
        /// - HTTP 404 NotFound：未找到对应配置。
        /// </returns>
        [HttpGet("configs/{id}")]
        public async Task<IActionResult> GetConfig(int id)
        {
            var config = await _context.AndonAlarmConfigs.FindAsync(id);
            if (config == null) return NotFound();
            return Ok(config);
        }
        /// <summary>
        /// 添加新的报警配置。(roleid = 6)
        /// </summary>
        /// <param name="config">待添加的 <see cref="AndonAlarmConfig"/> 对象，保存后将包含数据库生成的 Id。</param>
        /// <returns>
        /// - HTTP 200 OK 返回保存后的 <see cref="AndonAlarmConfig"/> 对象（包含 Id）。
        /// - HTTP 400 BadRequest：当传入模型验证失败时。
        /// </returns>
        [HttpPost("configs/create")]
        [Authorize(Roles ="6")]
        public async Task<IActionResult> AddConfig([FromBody] AndonAlarmConfig config)
        {
            _context.AndonAlarmConfigs.Add(config);
            await _context.SaveChangesAsync();
            return Ok(config);
        }

        /// <summary>
        /// 修改已有的报警配置，根据 ID 更新记录(roleid = 6)。
        /// </summary>
        /// <param name="id">报警配置 ID，必须与请求体中的 config.Id 一致。</param>
        /// <param name="config">包含更新数据的 <see cref="AndonAlarmConfig"/> 对象。</param>
        /// <returns>
        /// - HTTP 204 NoContent：更新成功，无返回体。
        /// - HTTP 400 BadRequest：路径 ID 与请求体 ID 不一致。
        /// - HTTP 404 NotFound：当目标记录不存在（在并发场景下可能发生）。
        /// </returns>
        [HttpPut("configs/update/{id}")]
        [Authorize(Roles ="6")]
        public async Task<IActionResult> UpdateConfig(int id, [FromBody] AndonAlarmConfig config)
        {
            if (id != config.Id) return BadRequest();
            _context.Entry(config).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 删除指定 ID 的报警配置。(roleid = 6)
        /// </summary>
        /// <param name="id">报警配置 ID。</param>
        /// <returns>
        /// - HTTP 204 NoContent：删除成功。
        /// - HTTP 404 NotFound：未找到对应配置。
        /// </returns>
        [HttpDelete("configs/delete/{id}")]
        [Authorize(Roles ="6")]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            var config = await _context.AndonAlarmConfigs.FindAsync(id);
            if (config == null) return NotFound();
            _context.AndonAlarmConfigs.Remove(config);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}