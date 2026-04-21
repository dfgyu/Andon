using Andon.Dtos;
using Andon.Models;
using Andon.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;   
using System;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;


namespace Andon.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AccountController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        /// <summary>
        ///  注册用户部分逻辑      
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto Dto) {
          //判断用户是否存在
          var exists = await _context.SysUsers
                .AnyAsync(u => u.Username == Dto.Username);
            if (exists)
            {
                return BadRequest("用户已经存在");
            }
            var pwdHash = BCrypt.Net.BCrypt.HashPassword(Dto.Password);
            var user = new SysUser
            {
                Username = Dto.Username,
                Password = pwdHash,
                RealName = Dto.RealName,
                Gender = Dto.Gender,
                Phone = Dto.Phone,
                Email = Dto.Email,
                RoleId = Dto.RoleId,
                IsEnabled = true
            };
            _context.SysUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { msg = "注册成功", userId = user.Id });
        }
        /// <summary>
        /// 用户登录部分逻辑
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto Dto)
        {
            var user = await _context.SysUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == Dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Dto.Password, user.Password))
            {
                return Unauthorized("用户名或密码错误");
            }
            if (!user.IsEnabled)
                return Unauthorized("账号已被禁用，请联系管理员");

            var token = JwtHelper.GenerateToken(user,_configuration);
            return Ok(new LoginResultDto
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                RoleId = user.RoleId,
                RoleName = user.Role!.RoleName,
                Token = token
            });
        }
        /// <summary>
        /// 获取用户信息部分逻辑
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _context.SysUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(user);
        }

        /// <summary>
        /// 获取用户列表部分逻辑
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(int page = 1, int limit = 10)
        {
            var query = _context.SysUsers
                .Include(u => u.Role)
                .AsNoTracking();

            var total = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items = users });
        }
        /// <summary>
        ///根据ID获取用户信息部分逻辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.SysUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound("用户不存在");
            return Ok(user);
        }

        /// <summary>
        /// 修改用户信息部分逻辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto Dto)
        {
            var user = await _context.SysUsers.FindAsync(id);
            if (user == null) return NotFound("用户不存在");

            user.RealName = Dto.RealName;
            user.Phone = Dto.Phone;
            user.Email = Dto.Email;
            user.Gender = Dto.Gender;
            user.RoleId = Dto.RoleId;

            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        /// <summary>
        /// 修改密码部分逻辑
        /// </summary>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpPut("change-pwd")]
        public async Task<IActionResult> ChangePwd(ChangePwdDto Dto)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _context.SysUsers.FindAsync(userId);

            if (!BCrypt.Net.BCrypt.Verify(Dto.OldPassword, user.Password))
                return BadRequest("原密码错误");

            user.Password = BCrypt.Net.BCrypt.HashPassword(Dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("密码修改成功");
        }

        /// <summary>
        /// 启用/禁用用户部分逻辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        [HttpPut("enable/{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> EnableUser(int id, [FromQuery] bool isEnabled)
        {

            var user = await _context.SysUsers.FindAsync(id);

            if (user == null) return NotFound("用户不存在");

            user.IsEnabled = isEnabled;
            await _context.SaveChangesAsync();

            return Ok(isEnabled ? "已启用" : "已禁用");
        }

        /// <summary>
        /// 删除用户部分逻辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.SysUsers.FindAsync(id);
            if (user == null) return NotFound("用户不存在");

            _context.SysUsers.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }

        /// <summary>
        /// 修改用户权限等级
        /// </summary>
        /// <param name="id">要修改的用户ID</param>
        /// <param name="newRoleId">新的权限角色ID</param>
        /// <returns></returns>
        [HttpPut("change-role/{id}")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromQuery] int newRoleId)
        {

            var user = await _context.SysUsers.FindAsync(id);
            if (user == null)
                return NotFound("用户不存在");

            user.RoleId = newRoleId;
            await _context.SaveChangesAsync();

            return Ok($"用户权限已修改为：{newRoleId}");
        }

        /// <summary>
        /// 根据姓名/用户名模糊查询用户
        /// </summary>
        /// <param name="keyword">搜索关键词（姓名/用户名）</param>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> SearchUsers(string keyword, int page = 1, int limit = 10)
        {

            var query = _context.SysUsers
                .Include(u => u.Role)
                .Where(u => u.Username.Contains(keyword) || u.RealName.Contains(keyword))
                .AsNoTracking();

            var total = await query.CountAsync();
            var list = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                total,
                items = list
            });
        }


    }
}
