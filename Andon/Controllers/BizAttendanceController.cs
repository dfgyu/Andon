/* 详细设计（伪代码）：
   目标：在现有的 BizAttendanceController 中为所有公开控制器方法增加完整的 XML 注释，
   并且在每个注释中添加 <response> 标签，明确描述返回值的数据结构。

   步骤：
   1. 在文件顶部添加一个多行注释，说明本次修改的目的（便于审阅）。
   2. 在控制器类前保留或添加 <summary> 注释，描述类职责。
   3. 对每个公开方法（GetMyAttendances, GetList, Search, Create, Update, Delete）：
      a. 添加 <summary> 概要描述（中文）。
      b. 为每个方法参数添加 <param> 注释（中文）。
      c. 添加 <returns> 注释指明返回 IActionResult。
      d. 添加一个或多个 <response> 标签，说明不同 HTTP 状态码时的返回数据结构。
         - 对于分页查询方法，<response code="200"> 描述包含 total(int) 和 items/List/array(实体或 DTO)。
         - 对于 Search，描述 total(int) 和 list(List<实体>)。
         - 对于 Create/Update/Delete，描述返回的成功消息字符串；并对可能的错误（400/404）添加描述。
      e. 保持方法体不变，确保编译通过。
   4. 保持已有 using 指令和命名空间不变。
   5. 格式化注释与代码，遵循现有代码风格（4 空格缩进）。

   输出：替换现有文件内容，包含伪代码注释和完整的 XML 注释。
*/

using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Andon.Controllers
{
    /// <summary>
    /// 员工考勤管理
    /// </summary>
    [ApiController]
    [Route("api/Attendance")]
    public class BizAttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BizAttendanceController(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取当前登录用户个人考勤记录(分页)(roleid = 7)
        /// </summary>
        /// <param name="page">页码（从1开始）</param>
        /// <param name="limit">每页条数</param>
        /// <returns>个人考勤列表封装为 IActionResult</returns>
        /// <response code="200">返回分页结果，结构为 { total: int, items: List&lt;BizAttendance&gt; }</response>
        [HttpGet("my")]
        [Authorize(Roles = "7")]
        public async Task<IActionResult> GetMyAttendances(int page = 1, int limit = 10)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var query = _context.BizAttendances
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.WorkDate)
                .AsNoTracking();

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items });
        }

        /// <summary>
        /// 获取全员考勤记录(无分页)
        /// </summary>
        /// <returns></returns>

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.BizAttendances
                .OrderByDescending(a => a.WorkDate)
                .AsNoTracking()
                .ToListAsync();
            return Ok(items);
        }


        /// <summary>
        /// 管理员获取全员考勤记录(分页)(roleid = 3)
        /// </summary>
        /// <param name="page">页码（从1开始）</param>
        /// <param name="limit">每页条数</param>
        /// <returns>全员考勤列表封装为 IActionResult</returns>
        /// <response code="200">返回分页结果，结构为 { total: int, items: List&lt;BizAttendance&gt; }</response>
        /// <response code="401">未授权访问（需要管理员角色）</response>
        [HttpGet("list")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> GetList(int page = 1, int limit = 10)
        {

            var query = _context.BizAttendances
                .OrderByDescending(a => a.WorkDate)
                .AsNoTracking();

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items });
        }

        /// <summary>
        /// 管理员多条件筛选考勤(用户ID/日期)(roleid = 3)
        /// </summary>
        /// <param name="dto">筛选条件（包括 Page、Limit、可选 UserId、可选 WorkDate）</param>
        /// <returns>筛选后考勤数据封装为 IActionResult</returns>
        /// <response code="200">返回筛选结果，结构为 { total: int, list: List&lt;BizAttendance&gt; }</response>
        /// <response code="401">未授权访问（需要管理员角色）</response>
        [HttpPost("search")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Search([FromBody] AttendanceSearchDto dto)
        {

            var query = _context.BizAttendances.AsQueryable();

            if (dto.UserId.HasValue)
                query = query.Where(a => a.UserId == dto.UserId.Value);

            if (!string.IsNullOrWhiteSpace(dto.WorkDate))
                query = query.Where(a => a.WorkDate.Contains(dto.WorkDate));

            if (dto.Status.HasValue)
                query = query.Where(a => a.Status == dto.Status.Value);

            query = query.OrderByDescending(a => a.WorkDate);

            var total = await query.CountAsync();
            var list = await query
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }

        /// <summary>
        /// 新增考勤打卡(roleid = 3)
        /// </summary>
        /// <param name="dto">考勤提交参数（WorkDate, StartWorkTime, EndWorkTime, Status, Remark）</param>
        /// <returns>提交结果封装为 IActionResult</returns>
        /// <response code="200">提交成功，返回字符串消息，例如 "考勤打卡提交成功"</response>
        /// <response code="400">BadRequest：例如重复打卡，返回错误字符串消息</response>
        [HttpPost("Create")]
        [Authorize(Roles ="3")]
        public async Task<IActionResult> Create([FromBody] AttendanceCreateDto dto)
        {

            var isExist = await _context.BizAttendances
                .AnyAsync(a => a.UserId == dto.UserId && a.WorkDate == dto.WorkDate);

            if (isExist)
                return BadRequest("请勿重复打卡");

            var entity = new BizAttendance
            {
                UserId = dto.UserId,
                WorkDate = dto.WorkDate,
                StartWorkTime = dto.StartWorkTime,
                EndWorkTime = dto.EndWorkTime,
                Status = dto.Status,
                Remark = dto.Remark
            };

            _context.BizAttendances.Add(entity);
            await _context.SaveChangesAsync();

            return Ok("考勤打卡提交成功");
        }

        /// <summary>
        /// 管理员编辑考勤记录(roleid = 3)
        /// </summary>
        /// <param name="id">考勤ID</param>
        /// <param name="dto">考勤编辑参数（WorkDate, StartWorkTime, EndWorkTime, Status, Remark）</param>
        /// <returns>修改结果封装为 IActionResult</returns>
        /// <response code="200">修改成功，返回字符串消息，例如 "考勤记录修改成功"</response>
        /// <response code="404">NotFound：指定 ID 的考勤记录不存在，返回错误字符串消息</response>
        /// <response code="401">未授权访问（需要管理员角色）</response>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Update(int id, [FromBody] AttendanceUpdateDto dto)
        {


            var attendance = await _context.BizAttendances.FindAsync(id);
            if (attendance == null)
                return NotFound("该条考勤记录不存在");

            attendance.WorkDate = dto.WorkDate;
            attendance.StartWorkTime = dto.StartWorkTime ?? attendance.StartWorkTime;
            attendance.EndWorkTime = dto.EndWorkTime ?? attendance.EndWorkTime;
            attendance.Status = dto.Status ?? attendance.Status;
            attendance.Remark = dto.Remark;

            await _context.SaveChangesAsync();
            return Ok("考勤记录修改成功");
        }

        /// <summary>
        /// 管理员删除指定考勤记录(roleid = 3)
        /// </summary>
        /// <param name="id">考勤ID</param>
        /// <returns>删除结果封装为 IActionResult</returns>
        /// <response code="200">删除成功，返回字符串消息，例如 "考勤记录删除成功"</response>
        /// <response code="404">NotFound：指定 ID 的考勤记录不存在，返回错误字符串消息</response>
        /// <response code="401">未授权访问（需要管理员角色）</response>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
        {


            var attendance = await _context.BizAttendances.FindAsync(id);
            if (attendance == null)
                return NotFound("该条考勤记录不存在");

            _context.BizAttendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return Ok("考勤记录删除成功");
        }
    }
}