using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{

    /// <summary>
    /// 报警接口：包含报警记录查询、处理、恢复，以及报警配置的CRUD接口
    /// </summary>
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

        // 报警列表
        /// <summary>
        /// 查询报警记录，支持按产线、设备、报警类型筛选，并分页返回总数和列表
        /// </summary>
        /// <param name="dto">报警查询条件</param>
        /// <returns></returns>
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
        /// 处理报警接口，接收报警ID和处理人ID，更新报警记录的处理人字段，并返回处理结果
        /// </summary>
        /// <param name="id">报警ID</param>
        /// <param name="handlerId">处理人ID</param>
        /// <returns></returns>
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
        /// 手动恢复接口，接收报警ID，更新报警记录的结束时间和持续时长，并返回恢复结果
        /// 没能自动恢复的报警，可以通过这个接口手动恢复，结束时间默认为当前时间，持续时长根据开始时间计算得出
        /// </summary>
        /// <param name="id">报警ID</param>
        /// <returns></returns>
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

        // ====================== 报警配置 CRUD ======================
        /// <summary>
        /// 获得报警配置列表接口，返回所有的报警配置记录，前端可以根据产线和报警类型进行筛选展示
        /// </summary>
        /// <returns></returns>
        [HttpGet("configs")]
        public async Task<IActionResult> GetConfigs()
        {
            var list = await _context.AndonAlarmConfigs.ToListAsync();
            return Ok(list);
        }
        /// <summary>
        /// 根据ID获得单个报警配置接口，返回对应ID的报警配置记录，前端可以用于编辑时加载数据展示在表单中
        /// </summary>
        /// <param name="id">报警配置ID</param>
        /// <returns></returns>
        [HttpGet("configs/{id}")]
        public async Task<IActionResult> GetConfig(int id)
        {
            var config = await _context.AndonAlarmConfigs.FindAsync(id);
            if (config == null) return NotFound();
            return Ok(config);
        }
        /// <summary>
        /// 添加报警配置接口，接收一个报警配置对象，保存到数据库，并返回保存后的对象（包含ID），前端可以通过这个接口新增报警配置
        /// </summary>
        /// <param name="config">报警配置对象</param>
        /// <returns></returns>
        [HttpPost("configs")]
        public async Task<IActionResult> AddConfig([FromBody] AndonAlarmConfig config)
        {
            _context.AndonAlarmConfigs.Add(config);
            await _context.SaveChangesAsync();
            return Ok(config);
        }

        /// <summary>
        /// 修改报警配置接口，接收一个包含ID的报警配置对象，根据ID更新数据库中的记录，并返回更新结果，前端可以通过这个接口修改已有的报警配置
        /// </summary>
        /// <param name="id">报警配置ID</param>
        /// <param name="config">报警配置对象 </param>
        /// <returns></returns>
        [HttpPut("configs/{id}")]
        public async Task<IActionResult> UpdateConfig(int id, [FromBody] AndonAlarmConfig config)
        {
            if (id != config.Id) return BadRequest();
            _context.Entry(config).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// 删除报警配置接口，接收一个报警配置ID，根据ID删除数据库中的记录，并返回删除结果，前端可以通过这个接口删除不需要的报警配置
        /// </summary>
        /// <param name="id">报警配置ID</param>
        /// <returns></returns>
        [HttpDelete("configs/{id}")]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            var config = await _context.AndonAlarmConfigs.FindAsync(id);
            if (config == null) return NotFound();
            _context.AndonAlarmConfigs.Remove(config);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}