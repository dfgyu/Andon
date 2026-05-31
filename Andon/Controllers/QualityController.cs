using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    /// <summary>
    /// 质检相关接口控制器。
    /// 需要认证，部分接口限制为管理员或操作员角色。
    /// </summary>
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
        /// 获得全部质检记录（不分页）。
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allQualityInspections = await _context.BizQualityInspections
                .AsNoTracking()
                .ToListAsync();
            return Ok(allQualityInspections);
        }

        /// <summary>
        /// 质检列表（分页）。
        /// </summary>
        /// <param name="page">页码，默认值为 1。</param>
        /// <param name="limit">每页条目数，默认值为 10。</param>
        /// <returns>
        /// 返回 HTTP 200 状态码，响应体为一个对象，结构如下：
        /// {
        ///   "total": int,                    // 符合条件的总记录数
        ///   "items": List&lt;BizQualityInspection&gt; // 当前页的质检记录列表
        /// }
        /// </returns>
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
        /// 搜索质检记录（支持按产品名、操作员、是否合格进行筛选），并支持分页。
        /// </summary>
        /// <param name="dto">包含搜索条件与分页信息的 DTO，例如 ProductName、OperatorId、IsQualified、Page、Limit。</param>
        /// <returns>
        /// 返回 HTTP 200 状态码，响应体为一个对象，结构如下：
        /// {
        ///   "total": int,                    // 符合条件的总记录数
        ///   "list": List&lt;BizQualityInspection&gt; // 当前页的质检记录列表（字段名为 list）
        /// }
        /// </returns>
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
        /// 获取指定 ID 的质检详情。
        /// </summary>
        /// <param name="id">质检记录 ID。</param>
        /// <returns>
        /// 成功时返回 HTTP 200，响应体为 <see cref="BizQualityInspection"/> 对象。
        /// 若记录不存在返回 HTTP 404。
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await _context.BizQualityInspections.FindAsync(id);
            if (model == null)
                return NotFound("质检记录不存在");

            return Ok(model);
        }

        /// <summary>
        /// 新增质检记录（roles = 5)
        /// </summary>
        /// <param name="dto">用于创建质检记录的 DTO，包含产品名、操作员、数量及合格信息等。</param>
        /// <returns>
        /// 成功时返回 HTTP 200，响应体为字符串消息，例如 "新增成功"。
        /// 可扩展为返回新建记录的 ID 或完整对象（当前实现仅返回消息）。
        /// </returns>
        [HttpPost("create")]
        [Authorize(Roles = "5")]
        public async Task<IActionResult> Create([FromBody] QualityCreateDto dto)
        {
            var model = new BizQualityInspection
            {
                ProductName = dto.ProductName,
                OperatorId = dto.OperatorId,
                TotalQty = dto.TotalQty,
                QualifiedQty = dto.QualifiedQty,
                UnqualifiedQty = dto.UnqualifiedQty,
                IsQualified = dto.IsQualified,
                DetectionDate = dto.DetectionDate 
            };

            _context.BizQualityInspections.Add(model);
            await _context.SaveChangesAsync();
            return Ok("新增成功");
        }

        /// <summary>
        /// 修改质检记录（roleid = 5
        /// </summary>
        /// <param name="id">要修改的质检记录 ID。</param>
        /// <param name="dto">包含要更新字段的 DTO。</param>
        /// <returns>
        /// 成功时返回 HTTP 200，并返回字符串消息 "修改成功"。
        /// 若记录不存在返回 HTTP 404。
        /// </returns>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "5")]
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
            model.DetectionDate = dto.DetectionDate;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 删除质检记录（roleid = 5)
        /// </summary>
        /// <param name="id">要删除的质检记录 ID。</param>
        /// <returns>
        /// 成功时返回 HTTP 200，并返回字符串消息 "删除成功"。
        /// 若记录不存在返回 HTTP 404。
        /// </returns>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "5")]
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