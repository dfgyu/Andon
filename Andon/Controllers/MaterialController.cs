using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    [Route("api/material")]
    [ApiController]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaterialController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 物料列表（分页）
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int limit = 10)
        {
            var query = _context.BizMaterials.AsNoTracking();
            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items });
        }

        /// <summary>
        /// 搜索物料（编码、名称、类型）
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] MaterialSearchDto dto)
        {
            var query = _context.BizMaterials.AsQueryable();

            if (!string.IsNullOrEmpty(dto.MaterialCode))
                query = query.Where(m => m.MaterialCode.Contains(dto.MaterialCode));

            if (!string.IsNullOrEmpty(dto.MaterialName))
                query = query.Where(m => m.MaterialName.Contains(dto.MaterialName));

            if (!string.IsNullOrEmpty(dto.Type))
                query = query.Where(m => m.Type == dto.Type);

            var total = await query.CountAsync();
            var list = await query
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }

        /// <summary>
        /// 物料详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var material = await _context.BizMaterials.FindAsync(id);
            if (material == null)
                return NotFound("物料不存在");

            return Ok(material);
        }

        /// <summary>
        /// 新增物料【管理员】
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Create([FromBody] MaterialCreateDto dto)
        {
            var material = new BizMaterial
            {
                MaterialCode = dto.MaterialCode,
                MaterialName = dto.MaterialName,
                Type = dto.Type,
                SurplusQty = dto.SurplusQty
            };

            _context.BizMaterials.Add(material);
            await _context.SaveChangesAsync();
            return Ok("新增成功");
        }

        /// <summary>
        /// 修改物料【管理员】
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Update(int id, [FromBody] MaterialUpdateDto dto)
        {
            var material = await _context.BizMaterials.FindAsync(id);
            if (material == null)
                return NotFound("物料不存在");

            material.MaterialCode = dto.MaterialCode;
            material.MaterialName = dto.MaterialName;
            material.Type = dto.Type;
            material.SurplusQty = dto.SurplusQty;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 修改库存【管理员】
        /// </summary>
        [HttpPatch("stock/{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> UpdateStock(int id, [FromQuery] int surplusQty)
        {
            var material = await _context.BizMaterials.FindAsync(id);
            if (material == null)
                return NotFound();

            material.SurplusQty = surplusQty;
            await _context.SaveChangesAsync();
            return Ok("库存已更新");
        }

        /// <summary>
        /// 删除物料【管理员】
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _context.BizMaterials.FindAsync(id);
            if (material == null)
                return NotFound("物料不存在");

            _context.BizMaterials.Remove(material);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}