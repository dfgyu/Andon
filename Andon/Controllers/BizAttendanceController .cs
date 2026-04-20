using Andon.Dtos;
using Andon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private bool IsAdmin()
        {
            var roleId = User.FindFirst("RoleId")?.Value;
            return roleId == "3";
        }

        /// <summary>
        /// 获取当前登录用户个人考勤记录(分页)
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <returns>个人考勤列表</returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAttendances(int page = 1, int limit = 10)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);

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
        /// 管理员获取全员考勤记录(分页)
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <returns>全员考勤列表</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int limit = 10)
        {
            if (!IsAdmin())
                return Forbid("权限不足");

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
        /// 管理员多条件筛选考勤(用户ID/日期)
        /// </summary>
        /// <param name="dto">筛选条件</param>
        /// <returns>筛选后考勤数据</returns>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] AttendanceSearchDto dto)
        {
            if (!IsAdmin())
                return Forbid("权限不足");

            var query = _context.BizAttendances.AsQueryable();

            if (dto.UserId.HasValue)
                query = query.Where(a => a.UserId == dto.UserId.Value);

            if (!string.IsNullOrWhiteSpace(dto.WorkDate))
                query = query.Where(a => a.WorkDate.Contains(dto.WorkDate));

            query = query.OrderByDescending(a => a.WorkDate);

            var total = await query.CountAsync();
            var list = await query
                .Skip((dto.Page - 1) * dto.Limit)
                .Take(dto.Limit)
                .ToListAsync();

            return Ok(new { total, list });
        }
        /// <summary>
        /// 员工提交当日考勤打卡
        /// </summary>
        /// <param name="dto">考勤提交参数</param>
        /// <returns>提交结果</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AttendanceCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);

            var isExist = await _context.BizAttendances
                .AnyAsync(a => a.UserId == userId && a.WorkDate == dto.WorkDate);

            if (isExist)
                return BadRequest("请勿重复打卡");

            var entity = new BizAttendance
            {
                UserId = userId,
                WorkDate = dto.WorkDate,
                WorkHours = dto.WorkHours,
                Remark = dto.Remark
            };

            _context.BizAttendances.Add(entity);
            await _context.SaveChangesAsync();

            return Ok("考勤打卡提交成功");
        }
        /// <summary>
        /// 管理员编辑考勤记录
        /// </summary>
        /// <param name="id">考勤ID</param>
        /// <param name="dto">考勤编辑参数</param>
        /// <returns>修改结果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AttendanceUpdateDto dto)
        {
            if (!IsAdmin())
                return Forbid("权限不足");

            var attendance = await _context.BizAttendances.FindAsync(id);
            if (attendance == null)
                return NotFound("该条考勤记录不存在");

            attendance.WorkDate = dto.WorkDate;
            attendance.WorkHours = dto.WorkHours;
            attendance.Remark = dto.Remark;

            await _context.SaveChangesAsync();
            return Ok("考勤记录修改成功");
        }
        /// <summary>
        /// 管理员删除指定考勤记录
        /// </summary>
        /// <param name="id">考勤ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return Forbid("权限不足");

            var attendance = await _context.BizAttendances.FindAsync(id);
            if (attendance == null)
                return NotFound("该条考勤记录不存在");

            _context.BizAttendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return Ok("考勤记录删除成功");
        }
    }
}
