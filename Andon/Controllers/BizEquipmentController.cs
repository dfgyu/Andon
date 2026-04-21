using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace Andon.Controllers
{
    [Route("api/equipment")]
    [ApiController]
    [Authorize]
    public class BizEquipmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BizEquipmentController(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 获取设备列表
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int limit = 10)
        {
            var query = _context.BizEquipments.AsNoTracking();
            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items });
        }

        /// <summary>
        /// 条件筛选设备
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] EquipmentSearchDto dto)
        {
            var query = _context.BizEquipments.AsQueryable();

            if (!string.IsNullOrEmpty(dto.EquipmentCode))
                query = query.Where(e => e.EquipmentCode.Contains(dto.EquipmentCode));

            if (!string.IsNullOrEmpty(dto.EquipmentName))
                query = query.Where(e => e.EquipmentName.Contains(dto.EquipmentName));

            if (!string.IsNullOrEmpty(dto.LineId))
                query = query.Where(e => e.LineId == dto.LineId);

            if (dto.Status.HasValue)
                query = query.Where(e => e.Status == dto.Status.Value);

            var total = await query.CountAsync();
            var list = await query
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }

        /// <summary>
        /// 获取单个设备
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var eq = await _context.BizEquipments.FindAsync(id);
            return eq == null ? NotFound("设备不存在") : Ok(eq);
        }

        /// <summary>
        /// 添加设备【管理员】
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Create([FromBody] EquipmentCreateDto dto)
        {
            var eq = new BizEquipment
            {
                EquipmentCode = dto.EquipmentCode,
                EquipmentName = dto.EquipmentName,
                LineId = dto.LineId,
                Status = dto.Status
            };

            _context.BizEquipments.Add(eq);
            await _context.SaveChangesAsync();
            return Ok("添加成功");
        }

        /// <summary>
        /// 修改设备【管理员】
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentUpdateDto dto)
        {
            var eq = await _context.BizEquipments.FindAsync(id);
            if (eq == null) return NotFound("设备不存在");

            eq.EquipmentCode = dto.EquipmentCode;
            eq.EquipmentName = dto.EquipmentName;
            eq.LineId = dto.LineId;
            eq.Status = dto.Status;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 修改设备状态【管理员 + 枚举】
        /// </summary>
        [HttpPatch("status/{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] Andon.Enums.EquipmentStatus status)
        {
            var eq = await _context.BizEquipments.FindAsync(id);
            if (eq == null) return NotFound();

            eq.Status = status;
            await _context.SaveChangesAsync();
            return Ok("状态已更新：" + status);
        }

        /// <summary>
        /// 删除设备【管理员】
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
        {
            var eq = await _context.BizEquipments.FindAsync(id);
            if (eq == null) return NotFound();

            _context.BizEquipments.Remove(eq);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}

