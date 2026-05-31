

using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    /// <summary>
    /// 物料管理接口。
    /// 提供物料的增删改查、分页查询、搜索和库存调整等功能。
    /// </summary>
    /// <remarks>
    /// 默认所有接口需要认证访问
    /// </remarks>
    [Route("api/material")]
    [ApiController]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        /// <summary>
        /// 应用程序数据库上下文，用于访问物料相关数据表。
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// 使用指定的数据库上下文构造 <see cref="MaterialController"/> 的实例。
        /// </summary>
        /// <param name="context">注入的 <see cref="AppDbContext"/> 实例。</param>
        public MaterialController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取全部物料列表（无分页）。
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allMaterials = await _context.BizMaterials
                .AsNoTracking()
                .ToListAsync();
            return Ok(allMaterials);
        }


        /// <summary>
        /// 获取物料列表（分页）。
        /// </summary>
        /// <param name="page">页码，默认值为 1。</param>
        /// <param name="limit">每页条数，默认值为 10。</param>
        /// <returns>
        /// 返回 200 OK，内容为一个 JSON 对象，结构如下：
        /// {
        ///   "total": int, // 总记录数
        ///   "items": BizMaterial[] // 当前页物料列表，数组元素类型为 <see cref="BizMaterial"/>
        /// }
        /// 可能的 HTTP 响应：
        /// - 200 OK: 返回上述对象。
        /// </returns>
        [HttpGet("list")]
        [Authorize]
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
        /// 根据编码、名称或类型搜索物料（支持分页）。
        /// </summary>
        /// <param name="dto">包含搜索条件和分页信息的 <see cref="MaterialSearchDto"/> 对象。</param>
        /// <returns>
        /// 返回 200 OK，内容为一个 JSON 对象，结构如下：
        /// {
        ///   "total": int, // 匹配的总条目数
        ///   "list": BizMaterial[] // 当前页物料列表，数组元素类型为 <see cref="BizMaterial"/>
        /// }
        /// 可能的 HTTP 响应：
        /// - 200 OK: 返回上述对象。
        /// </returns>
        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> Search([FromBody] MaterialSearchDto dto)
        {
            var query = _context.BizMaterials.AsQueryable();

            if (!string.IsNullOrEmpty(dto.MaterialCode))
                query = query.Where(m => EF.Functions.Like(m.MaterialCode, $"%{dto.MaterialCode}%"));

            if (!string.IsNullOrEmpty(dto.MaterialName))
                query = query.Where(m => EF.Functions.Like(m.MaterialName, $"%{dto.MaterialName}%"));

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
        /// 获取指定物料的详细信息。
        /// </summary>
        /// <param name="id">物料主键 ID。</param>
        /// <returns>
        /// 成功时返回 200 OK，响应体为 <see cref="BizMaterial"/> 实体，示例结构：
        /// {
        ///   "id": int,
        ///   "materialCode": string,
        ///   "materialName": string,
        ///   "type": string,
        ///   "surplusQty": int
        /// }
        /// 如果物料不存在，返回 404 NotFound，响应体可包含字符串消息 "物料不存在"。
        /// </returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var material = await _context.BizMaterials.FindAsync(id);
            if (material == null)
                return NotFound("物料不存在");

            return Ok(material);
        }

        /// <summary>
        /// 新增物料(roleid = 6)
        /// </summary>
        /// <param name="dto">包含待新增物料数据的 <see cref="MaterialCreateDto"/>。</param>
        /// <returns>
        /// 成功新增返回 200 OK，响应体为字符串消息，例如："新增成功"。
        /// 请求数据示例（dto）：
        /// {
        ///   "materialCode": string,
        ///   "materialName": string,
        ///   "type": string,
        ///   "surplusQty": int
        ///   "safetyMargin": int
        /// }
        /// 可能的 HTTP 响应：
        /// - 200 OK: 返回字符串提示。
        /// - 400/其他: 验证或处理错误由上层或全局异常处理返回。
        /// </returns>
        [HttpPost("Create")]
        [Authorize(Roles = "6")]
        public async Task<IActionResult> Create([FromBody] MaterialCreateDto dto)
        {
            var material = new BizMaterial
            {
                MaterialCode = dto.MaterialCode,
                MaterialName = dto.MaterialName,
                Type = dto.Type,
                SurplusQty = dto.SurplusQty,
                SafetyMargin = dto.SafetyMargin
            };

            _context.BizMaterials.Add(material);
            await _context.SaveChangesAsync();
            return Ok("新增成功");
        }

        /// <summary>
        /// 修改物料信息(roleid = 6)。
        /// </summary>
        /// <param name="id">要修改的物料主键 ID。</param>
        /// <param name="dto">包含更新字段的 <see cref="MaterialUpdateDto"/>。</param>
        /// <returns>
        /// 修改成功返回 200 OK，响应体为字符串消息，例如："修改成功"。
        /// 如果物料不存在，返回 404 NotFound，响应体可包含字符串消息 "物料不存在"。
        /// 请求数据示例（dto）：
        /// {
        ///   "materialCode": string,
        ///   "materialName": string,
        ///   "type": string,
        ///   "surplusQty": int
        ///   "safetyMargin": int
        /// }
        /// </returns>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "6")]
        public async Task<IActionResult> Update(int id, [FromBody] MaterialUpdateDto dto)
        {
            var material = await _context.BizMaterials.FindAsync(id);
            if (material == null)
                return NotFound("物料不存在");

            material.MaterialCode = dto.MaterialCode;
            material.MaterialName = dto.MaterialName;
            material.Type = dto.Type;
            material.SurplusQty = dto.SurplusQty;
            material.SafetyMargin = dto.SafetyMargin;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 修改物料库存（roleid = 6)。
        /// </summary>
        /// <param name="id">要调整库存的物料主键 ID。</param>
        /// <param name="surplusQty">新的剩余库存数量。</param>
        /// <returns>
        /// 成功更新返回 200 OK，响应体为字符串消息，例如："库存已更新"。
        /// 如果物料不存在，返回 404 NotFound。
        /// 请求示例（Query 参数）：
        /// ?surplusQty=123
        /// </returns>
        [HttpPatch("stock/{id}")]
        [Authorize(Roles = "6")]
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
        /// 删除物料（roleid = 6）。
        /// </summary>
        /// <param name="id">要删除的物料主键 ID。</param>
        /// <returns>
        /// 删除成功返回 200 OK，响应体为字符串消息，例如："删除成功"。
        /// 如果物料不存在，返回 404 NotFound，响应体可包含字符串消息 "物料不存在"。
        /// </returns>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "6")]
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