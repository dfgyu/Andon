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
        /// 获得全部工具列表（无分页）
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allTools = await _context.BizTools
                .AsNoTracking()
                .ToListAsync();
            return Ok(allTools);
        }



        /// <summary>
        /// 工具列表（分页）
        /// </summary>
        /// <param name="page">页码，默认值为 1</param>
        /// <param name="limit">每页条目数，默认值为 10</param>
        /// <returns>
        /// 返回 200 OK，内容为 JSON 对象：
        /// {
        ///   "total": int,                 // 符合条件的总条目数
        ///   "items": List&lt;BizTool&gt;  // 当前页的工具列表
        /// }
        /// </returns>
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
        /// <param name="dto">搜索条件，包含以下字段：
        /// ToolName (string) - 工具名称部分匹配，
        /// ToolModel (string) - 型号部分匹配，
        /// Warehouse (string) - 仓库精确匹配，
        /// Page (int) - 页码，
        /// Limit (int) - 每页条数
        /// </param>
        /// <returns>
        /// 返回 200 OK，内容为 JSON 对象：
        /// {
        ///   "total": int,                 // 符合条件的总条目数
        ///   "list": List&lt;BizTool&gt;   // 当前页的工具列表（字段名为 list）
        /// }
        /// </returns>
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
        /// <param name="id">工具 ID</param>
        /// <returns>
        /// 返回 200 OK，内容为单个 BizTool 对象；
        /// 如果未找到则返回 404 NotFound。
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tool = await _context.BizTools.FindAsync(id);
            if (tool == null)
                return NotFound("工具不存在");

            return Ok(tool);
        }

        /// <summary>
        /// 新增工具（roleid = 4 6）
        /// </summary>
        /// <param name="dto">创建 DTO，包含 ToolName, ToolModel, TotalQty, SurplusQty, MaintenanceDate, SafetyMargin, Warehouse</param>
        /// <returns>
        /// 返回 200 OK，内容为 string 消息：
        /// "新增成功"
        /// </returns>
        [HttpPost("create")]
        [Authorize(Roles = "4,6")]
        public async Task<IActionResult> Create([FromBody] ToolCreateDto dto)
        {
            var tool = new BizTool
            {
                ToolName = dto.ToolName,
                ToolModel = dto.ToolModel,
                TotalQty = dto.TotalQty,
                SurplusQty = dto.SurplusQty,
                Warehouse = dto.Warehouse,
                MaintenanceDate = dto.MaintenanceDate,
                SafetyMargin = dto.SafetyMargin
            };

            _context.BizTools.Add(tool);
            await _context.SaveChangesAsync();
            return Ok("新增成功");
        }

        /// <summary>
        /// 修改工具(roleid = 4 6)
        /// </summary>
        /// <param name="id">要修改的工具 ID</param>
        /// <param name="dto">更新 DTO，包含 ToolName, ToolModel, TotalQty, SurplusQty, MaintenanceDate, SafetyMargin, Warehouse</param>
        /// <returns>
        /// 返回 200 OK，内容为 string 消息：
        /// "修改成功"；
        /// 若未找到则返回 404 NotFound。
        /// </returns>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "4,6")]
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
            tool.MaintenanceDate = dto.MaintenanceDate;
            tool.SafetyMargin = dto.SafetyMargin;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 修改库存(roleid = 4 6)
        /// </summary>
        /// <param name="id">工具 ID</param>
        /// <param name="surplusQty">新的剩余库存数</param>
        /// <returns>
        /// 返回 200 OK，内容为 string 消息：
        /// "库存已更新"；
        /// 若未找到则返回 404 NotFound。
        /// </returns>
        [HttpPatch("stock/{id}")]
        [Authorize(Roles = "4,6")]
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
        /// 删除工具(roleid = 4,6)
        /// </summary>
        /// <param name="id">工具 ID</param>
        /// <returns>
        /// 返回 200 OK，内容为 string 消息：
        /// "删除成功"；
        /// 若未找到则返回 404 NotFound。
        /// </returns>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "4,6")]
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