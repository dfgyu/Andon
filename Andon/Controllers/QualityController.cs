using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    [Route("api/quality")]
    [ApiController]
    [Authorize]
    public class QualityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QualityController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 质检列表（分页）
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int limit = 10)
        {
            var query = _context.BizQualityInspections.AsNoTracking();
            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items });
        }

        /// <summary>
        /// 搜索质检记录（产品名/操作员/是否合格）
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] QualitySearchDto dto)
        {
            var query = _context.BizQualityInspections.AsQueryable();

            if (!string.IsNullOrEmpty(dto.ProductName))
                query = query.Where(q => q.ProductName.Contains(dto.ProductName));

            if (dto.OperatorId.HasValue)
                query = query.Where(q => q.OperatorId == dto.OperatorId);

            if (dto.IsQualified.HasValue)
                query = query.Where(q => q.IsQualified == dto.IsQualified);

            var total = await query.CountAsync();

            var list = await query
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }

        /// <summary>
        /// 质检详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await _context.BizQualityInspections.FindAsync(id);
            if (model == null)
                return NotFound("质检记录不存在");

            return Ok(model);
        }

        /// <summary>
        /// 新增质检记录【管理员/操作员】
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "2,3")]
        public async Task<IActionResult> Create([FromBody] QualityCreateDto dto)
        {
            var model = new BizQualityInspection
            {
                ProductName = dto.ProductName,
                OperatorId = dto.OperatorId,
                TotalQty = dto.TotalQty,
                QualifiedQty = dto.QualifiedQty,
                UnqualifiedQty = dto.UnqualifiedQty,
                IsQualified = dto.IsQualified
            };

            _context.BizQualityInspections.Add(model);
            await _context.SaveChangesAsync();
            return Ok("新增成功");
        }

        /// <summary>
        /// 修改质检记录【管理员】
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Update(int id, [FromBody] QualityUpdateDto dto)
        {
            var model = await _context.BizQualityInspections.FindAsync(id);
            if (model == null)
                return NotFound("质检记录不存在");

            model.ProductName = dto.ProductName;
            model.OperatorId = dto.OperatorId;
            model.TotalQty = dto.TotalQty;
            model.QualifiedQty = dto.QualifiedQty;
            model.UnqualifiedQty = dto.UnqualifiedQty;
            model.IsQualified = dto.IsQualified;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 删除质检记录【管理员】
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.BizQualityInspections.FindAsync(id);
            if (model == null)
                return NotFound("质检记录不存在");

            _context.BizQualityInspections.Remove(model);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}