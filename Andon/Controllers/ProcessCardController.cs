using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    [Route("api/processcard")]
    [ApiController]
    [Authorize] // 登录即可访问
    public class ProcessCardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProcessCardController(AppDbContext context)
        {
            _context = context;
        }

        // ==============================
        // 列表 + 分页 + 筛选
        // ==============================
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] ProcessCardSearchDto dto)
        {
            var query = _context.BizProcessCards
                .Include(e => e.BizEquipment)        // 关联设备
                .Include(u => u.SysUser)             // 关联用户
                .AsQueryable();

            // 筛选
            if (!string.IsNullOrEmpty(dto.CardCode))
                query = query.Where(p => p.CardCode.Contains(dto.CardCode));

            if (!string.IsNullOrEmpty(dto.ProcessName))
                query = query.Where(p => p.ProcessName.Contains(dto.ProcessName));

            if (dto.EquipmentId.HasValue)
                query = query.Where(p => p.EquipmentId == dto.EquipmentId);

            if (dto.OperatorId.HasValue)
                query = query.Where(p => p.OperatorId == dto.OperatorId);

            // 总数
            var total = await query.CountAsync();

            // 分页
            var items = await query
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .Select(p => new ProcessCardDto
                {
                    Id = p.Id,
                    CardCode = p.CardCode,
                    ProcessName = p.ProcessName,
                    EquipmentId = p.EquipmentId,
                    EquipmentName = p.BizEquipment != null ? p.BizEquipment.EquipmentName : "",
                    OperatorId = p.OperatorId,
                    OperatorName = p.SysUser != null ? p.SysUser.Username : "",
                    EstTime = p.EstTime
                })
                .ToListAsync();

            return Ok(new { total, items });
        }

        // ==============================
        // 单个详情
        // ==============================
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _context.BizProcessCards
                .Include(e => e.BizEquipment)
                .Include(u => u.SysUser)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return NotFound("工序卡不存在");

            var dto = new ProcessCardDto
            {
                Id = p.Id,
                CardCode = p.CardCode,
                ProcessName = p.ProcessName,
                EquipmentId = p.EquipmentId,
                EquipmentName = p.BizEquipment != null ? p.BizEquipment.EquipmentName : "",
                OperatorId = p.OperatorId,
                OperatorName = p.SysUser != null ? p.SysUser.Username : "",
                EstTime = p.EstTime
            };

            return Ok(dto);
        }

        // ==============================
        // 新增（管理员权限）
        // ==============================
        [HttpPost]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Create([FromBody] ProcessCardCreateDto dto)
        {
            var card = new BizProcessCard
            {
                CardCode = dto.CardCode,
                ProcessName = dto.ProcessName,
                EquipmentId = dto.EquipmentId,
                OperatorId = dto.OperatorId,
                EstTime = dto.EstTime
            };

            _context.BizProcessCards.Add(card);
            await _context.SaveChangesAsync();

            return Ok("创建成功");
        }

        // ==============================
        // 修改（管理员权限）
        // ==============================
        [HttpPut("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Update(int id, [FromBody] ProcessCardCreateDto dto)
        {
            var card = await _context.BizProcessCards.FindAsync(id);
            if (card == null) return NotFound("工序卡不存在");

            card.CardCode = dto.CardCode;
            card.ProcessName = dto.ProcessName;
            card.EquipmentId = dto.EquipmentId;
            card.OperatorId = dto.OperatorId;
            card.EstTime = dto.EstTime;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        // ==============================
        // 删除（管理员权限）
        // ==============================
        [HttpDelete("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
        {
            var card = await _context.BizProcessCards.FindAsync(id);
            if (card == null) return NotFound("工序卡不存在");

            _context.BizProcessCards.Remove(card);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}