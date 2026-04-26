using Andon.Dtos;
using Andon.Enums;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace Andon.Controllers
{
    [Route("api/linemonitor")]
    [ApiController]
    [Authorize] // 登录即可访问
    public class LineMonitorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LineMonitorController(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 获取全产线实时监控数据
        /// </summary>
        [HttpPost("data")]
        public async Task<IActionResult> GetLineMonitorData([FromBody] LineMonitorSearchDto dto)
        {
            // 主表：设备
            var query = _context.BizEquipments.AsQueryable();

            // 筛选
            if (!string.IsNullOrEmpty(dto.LineId))
                query = query.Where(e => e.LineId == dto.LineId);

            if (dto.EquipmentId.HasValue)
                query = query.Where(e => e.Id == dto.EquipmentId);

            // 关联：最新IoT数据 + 最新工序卡
            var dataList = await query
                .Select(e => new LineMonitorDto
                {
                    EquipmentId = e.Id,
                    EquipmentCode = e.EquipmentCode,
                    EquipmentName = e.EquipmentName,
                    LineId = e.LineId,
                    ProcessName = e.Process.ToString(),

                    // 最新IoT实时状态
                    RunStatus = _context.BizIotEquipmentDatas
                        .Where(i => i.LineId == e.LineId)
                        .OrderByDescending(i => i.CollectionTime)
                        .Select(i => i.RunStatus)
                        .FirstOrDefault(),

                    IsBlocked = _context.BizIotEquipmentDatas
                        .Where(i => i.LineId == e.LineId)
                        .OrderByDescending(i => i.CollectionTime)
                        .Select(i => i.IsBlocked)
                        .FirstOrDefault(),

                    IsOverheat = _context.BizIotEquipmentDatas
                        .Where(i => i.LineId == e.LineId)
                        .OrderByDescending(i => i.CollectionTime)
                        .Select(i => i.IsOverheat)
                        .FirstOrDefault(),

                    IsDeviation = _context.BizIotEquipmentDatas
                        .Where(i => i.LineId == e.LineId)
                        .OrderByDescending(i => i.CollectionTime)
                        .Select(i => i.IsDeviation)
                        .FirstOrDefault(),

                    IsPackError = _context.BizIotEquipmentDatas
                        .Where(i => i.LineId == e.LineId)
                        .OrderByDescending(i => i.CollectionTime)
                        .Select(i => i.IsPackError)
                        .FirstOrDefault(),

                    CollectionTime = _context.BizIotEquipmentDatas
                        .Where(i => i.LineId == e.LineId)
                        .OrderByDescending(i => i.CollectionTime)
                        .Select(i => i.CollectionTime)
                        .FirstOrDefault(),

                    // 当前工序卡
                    ProcessCardId = _context.BizProcessCards
                        .Where(c => c.EquipmentId == e.Id)
                        .OrderByDescending(c => c.Id)
                        .Select(c => c.Id)
                        .FirstOrDefault(),

                    CardCode = _context.BizProcessCards
                        .Where(c => c.EquipmentId == e.Id)
                        .OrderByDescending(c => c.Id)
                        .Select(c => c.CardCode)
                        .FirstOrDefault(),

                    // 操作员
                    OperatorName = _context.BizProcessCards
                        .Where(c => c.EquipmentId == e.Id)
                        .OrderByDescending(c => c.Id)
                        .Select(c => c.SysUser.Username)
                        .FirstOrDefault()
                })
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            // 补充状态文本 + 报警判断
            foreach (var item in dataList)
            {
                item.RunStatusText = item.RunStatus switch
                {
                    0 => "正常",
                    1 => "待机",
                    2 => "故障",
                    3 => "报警中",
                    _ => "离线"
                };

                // 报警判断逻辑
                bool isAlarm =
                    item.RunStatus == (int)IotRunStatus.故障 ||
                    item.RunStatus == (int)IotRunStatus.报警中 ||
                    item.IsBlocked ||
                    item.IsOverheat ||
                    item.IsDeviation ||
                    item.IsPackError;

                item.IsAlarm = isAlarm;

                // 报警提示
                if (isAlarm)
                {
                    var msg = new List<string>();
                    if (item.IsBlocked) msg.Add("堵料");
                    if (item.IsOverheat) msg.Add("过热");
                    if (item.IsDeviation) msg.Add("跑偏");
                    if (item.IsPackError) msg.Add("包装异常");
                    if (item.RunStatus is 2 or 3) msg.Add("设备异常");

                    item.AlarmMessage = string.Join("，", msg);
                }
            }

            // 报警过滤
            if (dto.IsAlarm.HasValue)
                dataList = dataList.Where(d => d.IsAlarm == dto.IsAlarm.Value).ToList();

            return Ok(new
            {
                total = dataList.Count,
                items = dataList
            });
        }

        /// <summary>
        /// 获取单台设备详细实时监控
        /// </summary>
        [HttpGet("equipment/{equipmentId}")]
        public async Task<IActionResult> GetEquipmentMonitor(int equipmentId)
        {
            var eq = await _context.BizEquipments.FindAsync(equipmentId);
            if (eq == null) return NotFound("设备不存在");

            var dto = new LineMonitorDto
            {
                EquipmentId = eq.Id,
                EquipmentCode = eq.EquipmentCode,
                EquipmentName = eq.EquipmentName,
                LineId = eq.LineId,
                ProcessName = eq.Process.ToString()
            };

            // 最新IoT
            var iot = await _context.BizIotEquipmentDatas
                .Where(i => i.LineId == eq.LineId)
                .OrderByDescending(i => i.CollectionTime)
                .FirstOrDefaultAsync();

            if (iot != null)
            {
                dto.RunStatus = iot.RunStatus;
                dto.IsBlocked = iot.IsBlocked;
                dto.IsOverheat = iot.IsOverheat;
                dto.IsDeviation = iot.IsDeviation;
                dto.IsPackError = iot.IsPackError;
                dto.CollectionTime = iot.CollectionTime;
            }

            // 最新工序卡
            var card = await _context.BizProcessCards
                .Where(c => c.EquipmentId == eq.Id)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (card != null)
            {
                dto.ProcessCardId = card.Id;
                dto.CardCode = card.CardCode;
                dto.OperatorName = await _context.SysUsers
                    .Where(u => u.Id == card.OperatorId)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();
            }

            // 报警
            dto.IsAlarm =
                dto.RunStatus is 2 or 3 ||
                dto.IsBlocked ||
                dto.IsOverheat ||
                dto.IsDeviation ||
                dto.IsPackError;

            return Ok(dto);
        }
    }
}
