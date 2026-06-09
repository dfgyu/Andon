using Andon.Dtos;
using Andon.Enums;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Andon.Controllers
{
    /// <summary>
    /// 设备管理控制器
    /// </summary>
    public class BizEquipmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BizEquipmentController> _logger;

        public BizEquipmentController(AppDbContext context, ILogger<BizEquipmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 获取全部设备列表（无分页）
        /// </summary>
        /// <response>返回数据结构：List&lt;BizEquipment&gt;</response>
        /// <response code="200">查询成功，返回全部设备列表</response>
        [HttpGet("api/bizequipment/all")]
        public async Task<IActionResult> GetAll()
        {
            var query = _context.BizEquipments.AsNoTracking().AsQueryable();

            // 打印生成的 SQL 便于比对
            try { _logger.LogDebug("GetAll SQL: {Sql}", query.ToQueryString()); } catch { }

            var allEquipments = await query
                .Select(e => new
                {
                    e.Id,
                    e.EquipmentCode,
                    e.EquipmentName,
                    Process = (int)e.Process,
                    e.LineId,
                    Status = (int)e.Status, // 明确返回数据库中的整数值
                    e.AlertContact,
                    e.InstallationDate,
                    e.MaintenanceDate,
                    e.EquipmentModel
                })
                .ToListAsync();

            // 记录每条设备的 status，便于对比 logger 写入的值
            try
            {
                foreach (var eq in allEquipments)
                {
                    _logger.LogInformation("GetAll 返回设备 {Id} status={Status}", eq.Id, eq.Status);
                }
            }
            catch { }

            return Ok(allEquipments);
        }

        /// <summary>
        /// 获取设备列表（分页）
        /// </summary>
        /// <param name="page">页码，默认 1</param>
        /// <param name="limit">每页数量，默认 10</param>
        /// <response>返回数据结构：{"total": int, "items": List&lt;BizEquipment&gt;}</response>
        /// <response code="200">查询成功，返回分页后的设备总数与设备列表</response>
        [HttpGet("api/bizequipment/list")]
        public async Task<IActionResult> GetList(int page = 1, int limit = 10)
        {
            var query = _context.BizEquipments.AsNoTracking();
            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(e => new
                {
                    e.Id,
                    e.EquipmentCode,
                    e.EquipmentName,
                    Process = (int)e.Process,
                    e.LineId,
                    Status = (int)e.Status,
                    e.AlertContact,
                    e.InstallationDate,
                    e.MaintenanceDate,
                    e.EquipmentModel
                })
                .ToListAsync();

            try
            {
                _logger.LogDebug("GetList SQL: {Sql}", query.ToQueryString());
                foreach (var it in items)
                {
                    _logger.LogInformation("GetList 返回设备 {Id} status={Status}", it.Id, it.Status);
                }
            }
            catch { }

            return Ok(new { total, items });
        }

        /// <summary>
        /// 条件筛选设备（编码、名称、产线、状态、工序、联系人），支持分页
        /// </summary>
        /// <param name="dto">查询条件及分页参数</param>
        /// <response>返回数据结构：{"total": int, "list": List&lt;BizEquipment&gt;}</response>
        /// <response code="200">查询成功，返回符合条件的设备总数与当前页设备列表</response>

        [HttpPost("api/bizequipment/search")]
        public async Task<IActionResult> Search([FromBody] EquipmentSearchDto dto)
        {
            // 即使不传 dto 也不报错
            dto ??= new EquipmentSearchDto();

            // 分页默认值
            int page = dto.Page < 1 ? 1 : dto.Page;
            int limit = dto.Limit < 1 ? 10 : dto.Limit;

            var query = _context.BizEquipments

                     .AsNoTracking()       
                     .IgnoreAutoIncludes() 
                     .AsQueryable();

            var sql = query.ToQueryString();
            Console.WriteLine(sql);

            
            if (!string.IsNullOrWhiteSpace(dto.EquipmentCode))
                query = query.Where(e => EF.Functions.Like(e.EquipmentCode, $"%{dto.EquipmentCode}%"));

            if (!string.IsNullOrWhiteSpace(dto.EquipmentName))
                query = query.Where(e => EF.Functions.Like(e.EquipmentName, $"%{dto.EquipmentName}%"));

            if (!string.IsNullOrWhiteSpace(dto.EquipmentModel))
                query = query.Where(e => EF.Functions.Like(e.EquipmentModel, $"%{dto.EquipmentModel}%"));

            if (!string.IsNullOrWhiteSpace(dto.LineId))
                query = query.Where(e => e.LineId == dto.LineId);

            if (dto.Status.HasValue)
                query = query.Where(e => (int)e.Status == (int)dto.Status.Value);

            if (dto.Process.HasValue)
                query = query.Where(e => (int)e.Process == (int)dto.Process.Value);

            if (!string.IsNullOrWhiteSpace(dto.AlertContact))
                query = query.Where(e => e.AlertContact != null && e.AlertContact.Contains(dto.AlertContact));

            var total = await query.CountAsync();
            var list = await query
                 .Select(e => new
                 {
                     e.Id,
                     e.EquipmentCode,
                     e.EquipmentName,
                     Process = (int)e.Process,
                     e.LineId,
                     Status = (int)e.Status,
                     e.AlertContact,
                     e.InstallationDate,
                     e.MaintenanceDate,
                     e.EquipmentModel
                 })
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();


            return Ok(new { total, list });


        }

        /// <summary>
        /// 获取单个设备详情
        /// </summary>
        /// <param name="id">设备主键 Id</param>
        /// <response>返回数据结构：BizEquipment</response>
        /// <response code="200">查询成功，返回设备对象</response>
        /// <response code="404">设备不存在，返回错误信息字符串</response>
        [HttpGet("api/bizequipment/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var eq = await _context.BizEquipments
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.EquipmentCode,
                    e.EquipmentName,
                    Process = (int)e.Process,
                    e.LineId,
                    Status = (int)e.Status,
                    e.AlertContact,
                    e.InstallationDate,
                    e.MaintenanceDate,
                    e.EquipmentModel
                })
                .FirstOrDefaultAsync();

            if (eq != null)
            {
                try { _logger.LogInformation("GetById 返回设备 {Id} status={Status}", eq.Id, eq.Status); } catch { }
                return Ok(eq);
            }

            return NotFound("设备不存在");
        }

        /// <summary>
        /// 添加设备(roleid = 3 ,4,6)
        /// </summary>
        /// <param name="dto">设备创建 DTO</param>
        /// <response>返回数据结构：string（操作结果消息）</response>
        /// <response code="200">添加成功，返回成功消息</response>
        [HttpPost("api/bizequipment/Create")]

        [Authorize(Roles = "3,4,6")]
        public async Task<IActionResult> Create([FromBody] EquipmentCreateDto dto)
        {
            var eq = new BizEquipment
            {
                EquipmentCode = dto.EquipmentCode,
                EquipmentName = dto.EquipmentName,
                LineId = dto.LineId,
                Status = dto.Status,
                Process = dto.Process,          // 工序
                AlertContact = dto.AlertContact, // 报警联系人
                InstallationDate = dto.InstallationDate,
                MaintenanceDate = dto.MaintenanceDate,
                EquipmentModel = dto.EquipmentModel
            };

            _logger.LogInformation("Create equipment Id(temp) status={Status}", (int)eq.Status);
            _context.BizEquipments.Add(eq);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created equipment Id={Id} status={Status}", eq.Id, (int)eq.Status);

            return Ok("添加成功");
        }

        /// <summary>
        /// 修改设备（roleid = 3 4 6）
        /// </summary>
        /// <param name="id">设备 Id</param>
        /// <param name="dto">设备更新 DTO</param>
        /// <response>返回数据结构：string（操作结果消息）</response>
        /// <response code="200">修改成功，返回成功消息</response>
        /// <response code="404">设备不存在，返回错误信息字符串</response>
        [HttpPut("api/bizequipment/update/{id}")]
        [Authorize(Roles = "3,4,6")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentUpdateDto dto)
        {
            var eq = await _context.BizEquipments.FindAsync(id);
            if (eq == null) return NotFound("设备不存在");

            _logger.LogInformation("Update equipment {Id} before status={Old}", id, (int)eq.Status);
            eq.EquipmentCode = dto.EquipmentCode;
            eq.EquipmentName = dto.EquipmentName;
            eq.LineId = dto.LineId;
            eq.Status = dto.Status;
            eq.Process = dto.Process;          
            eq.AlertContact = dto.AlertContact; 
            eq.InstallationDate = dto.InstallationDate;
            eq.MaintenanceDate = dto.MaintenanceDate;
            eq.EquipmentModel = dto.EquipmentModel;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Update equipment {Id} after status={New}", id, (int)eq.Status);

            return Ok("修改成功");
        }

        /// <summary>
        /// 修改设备状态（roleid = 3 4 6）
        /// </summary>
        /// <param name="id">设备 Id</param>
        /// <param name="status">新状态</param>
        /// <response>返回数据结构：string（操作结果消息，包含新状态）</response>
        /// <response code="200">状态更新成功，返回包含状态的消息</response>
        /// <response code="404">设备不存在</response>
        [HttpPatch("api/bizequipment/status/{id}")]
        [Authorize(Roles = "3,4,6")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] EquipmentStatus status)
        {
            var eq = await _context.BizEquipments.FindAsync(id);
            if (eq == null) return NotFound();

            _logger.LogInformation("UpdateStatus request for equipment {Id} from {Old} to {New}", id, (int)eq.Status, (int)status);
            eq.Status = status;
            await _context.SaveChangesAsync();
            _logger.LogInformation("UpdateStatus applied for equipment {Id} now {New}", id, (int)eq.Status);

            return Ok("状态已更新：" + status);
        }

        /// <summary>
        /// 删除设备（roleid = 3 4 6）
        /// </summary>
        /// <param name="id">设备 Id</param>
        /// <response>返回数据结构：string（操作结果消息）</response>
        /// <response code="200">删除成功</response>
        /// <response code="404">设备不存在</response>
        [HttpDelete("api/bizequipment/delete/{id}")]
        [Authorize(Roles = "3,4,6")]
        public async Task<IActionResult> Delete(int id)
        {
            var eq = await _context.BizEquipments.FindAsync(id);
            if (eq == null) return NotFound();

            _context.BizEquipments.Remove(eq);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }

        /// <summary>
        /// debug
        /// </summary>
        /// <param name="id">设备主键 Id</param>
        /// <response>返回数据结构：{"efStatus": int?, "rawDbStatus": int?, "sql": string, "error": string}</response>
        /// <response code="200">查询成功，返回设备状态及原始数据库状态</response>
        [HttpGet("api/bizequipment/debug/{id}")]
        public async Task<IActionResult> DebugStatus(int id)
        {
            var efResult = await _context.BizEquipments
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new { Status = (int)e.Status })
                .FirstOrDefaultAsync();

            int? rawDbStatus = null;
            string rawSql = null;

            try
            {
                try { rawSql = _context.BizEquipments.Where(e => e.Id == id).ToQueryString(); } catch { }

                var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT `status` FROM `biz_equipment` WHERE `id` = @id";
                var p = cmd.CreateParameter();
                p.ParameterName = "@id";
                p.Value = id;
                cmd.Parameters.Add(p);

                var obj = await cmd.ExecuteScalarAsync();
                if (obj != null && obj != DBNull.Value)
                    rawDbStatus = Convert.ToInt32(obj);

                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                return Ok(new { error = ex.Message, efStatus = efResult?.Status, rawDbStatus, sql = rawSql });
            }

            return Ok(new { efStatus = efResult?.Status, rawDbStatus, sql = rawSql });
        }
    }
}

