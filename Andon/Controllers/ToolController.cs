using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    /// <summary>
    /// 工具管理接口
    /// </summary>
    [Route("api/tool")]
    [ApiController]
    [Authorize]
    public class ToolController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ToolController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 工具列表（分页）
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int limit = 10)
        {
            var query = _context.BizTools.AsNoTracking();
            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items });
        }

        /// <summary>
        /// 搜索工具（名称/型号/仓库）
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] ToolSearchDto dto)
        {
            var query = _context.BizTools.AsQueryable();

            if (!string.IsNullOrEmpty(dto.ToolName))
                query = query.Where(t => t.ToolName.Contains(dto.ToolName));

            if (!string.IsNullOrEmpty(dto.ToolModel))
                query = query.Where(t => t.ToolModel.Contains(dto.ToolModel));

            if (!string.IsNullOrEmpty(dto.Warehouse))
                query = query.Where(t => t.Warehouse == dto.Warehouse);

            var total = await query.CountAsync();

            var list = await query
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }

        /// <summary>
        /// 工具详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tool = await _context.BizTools.FindAsync(id);
            if (tool == null)
                return NotFound("工具不存在");

            return Ok(tool);
        }

        /// <summary>
        /// 新增工具【管理员】
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Create([FromBody] ToolCreateDto dto)
        {
            var tool = new BizTool
            {
                ToolName = dto.ToolName,
                ToolModel = dto.ToolModel,
                TotalQty = dto.TotalQty,
                SurplusQty = dto.SurplusQty,
                Warehouse = dto.Warehouse
            };

            _context.BizTools.Add(tool);
            await _context.SaveChangesAsync();
            return Ok("新增成功");
        }

        /// <summary>
        /// 修改工具【管理员】
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Update(int id, [FromBody] ToolUpdateDto dto)
        {
            var tool = await _context.BizTools.FindAsync(id);
            if (tool == null)
                return NotFound("工具不存在");

            tool.ToolName = dto.ToolName;
            tool.ToolModel = dto.ToolModel;
            tool.TotalQty = dto.TotalQty;
            tool.SurplusQty = dto.SurplusQty;
            tool.Warehouse = dto.Warehouse;

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
            var tool = await _context.BizTools.FindAsync(id);
            if (tool == null)
                return NotFound();

            tool.SurplusQty = surplusQty;
            await _context.SaveChangesAsync();
            return Ok("库存已更新");
        }

        /// <summary>
        /// 删除工具【管理员】
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
        {
            var tool = await _context.BizTools.FindAsync(id);
            if (tool == null)
                return NotFound("工具不存在");

            _context.BizTools.Remove(tool);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}