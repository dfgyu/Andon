    using Andon.Dtos;
    using Andon.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace Andon.Controllers
    {
        /// <summary>
        /// 工序卡逻辑
        /// </summary>
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

            /// <summary>
            /// 工序卡列表
            /// </summary>
            /// <param name="dto">工序卡查询条件，包含分页和筛选字段</param>
            /// <returns>
            /// 返回包含总数和列表的对象。
            /// JSON 结构:
            /// {
            ///   "total": int,                  // 满足筛选条件的总记录数
            ///   "items": ProcessCardDto[]      // 当前页的工序卡 DTO 列表
            /// }
            /// 其中 ProcessCardDto 字段包括:
            /// - Id (int)
            /// - CardCode (string)
            /// - ProcessName (string)
            /// - EquipmentId (int?)
            /// - EquipmentName (string)
            /// - OperatorId (int?)
            /// - OperatorName (string)
            /// - EstTime (int?)
            /// </returns>
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

            /// <summary>
            /// 单个工序详情
            /// </summary>
            /// <param name="id">工序卡ID</param>
            /// <returns>
            /// 返回单个工序卡的 DTO。
            /// JSON 结构对应 ProcessCardDto:
            /// {
            ///   "Id": int,
            ///   "CardCode": string,
            ///   "ProcessName": string,
            ///   "EquipmentId": int?,
            ///   "EquipmentName": string,
            ///   "OperatorId": int?,
            ///   "OperatorName": string,
            ///   "EstTime": int?
            /// }
            /// 若不存在则返回 404 状态码。
            /// </returns>
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

            /// <summary>
            /// 新增工序卡
            /// </summary>
            /// <param name="dto">工序卡创建信息</param>
            /// <returns>返回操作结果消息，类型为 string（例如 "创建成功"）。</returns>
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

            /// <summary>
            /// 修改工序卡
            /// </summary>
            /// <param name="id">工序卡ID</param>
            /// <param name="dto">工序卡更新信息</param>
            /// <returns>返回操作结果消息，类型为 string（例如 "修改成功"）。若记录不存在则返回 404。</returns>
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

            /// <summary>
            /// 删除工序卡
            /// </summary>
            /// <param name="id">工序卡ID</param>
            /// <returns>返回操作结果消息，类型为 string（例如 "删除成功"）。若记录不存在则返回 404。</returns>
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