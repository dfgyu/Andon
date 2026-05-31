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
using Azure.Identity;
using Microsoft.VisualBasic;


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
        /// 通过id获取用户名部分逻辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getnamebyid/{id}")]
        public async Task<IActionResult> GetNameById(int id)
        {
            var user = await _context.SysUsers.FindAsync(id);
            if (user == null) return NotFound("用户不存在");
            return Ok(new { username = user.Username });
        }

        /// <summary>
        ///  注册用户部分逻辑      
        /// </summary>
        /// <param name="Dto"></param>
        /// <response code="200">
        /// 返回用户注册信息<br/>
        /// {<br/>
        ///    userId:用户id <br/>
        /// }
        /// </response>
        /// 
        /// <returns></returns>

        [HttpPost("register")]
        [Authorize(Roles ="6")]
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
        /// <response code="200">
        /// 返回用户登录信息<br/>
        /// {<br/>
        ///    Id 用户id<br/>
        ///    Username 用户名<br/>
        ///    RealName 用户真名<br/>
        ///    RoleId 用户权限id<br/>
        ///    RoleName 根据权限id查权限名<br/>
        ///    Token 令牌<br/>
        ///    
        /// }
        /// </response>
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
                return Ok(new
                {
                    code = 4003,
                    msg = "用户名或密码错误"
                }                     
                   );
            }
            if (!user.IsEnabled)
                return Ok(new 
                {
                    code = 4004,
                    msg = "用户被禁用"
                }                        
                    );

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
        /// 获取用户信息部分逻辑(roleid = 7)
        /// </summary>
        /// <response code="200">
        /// 返回用户信息
        /// {<br/>
        ///     包含user实体类中的全部内容<br/>
        ///    Id 用户id<br/>
        ///    Username 用户名<br/>
        ///    Password 经hash加密过的密码 可以无视<br/>
        ///    RealName 用户真名<br/>
        ///    Email 电子邮件<br/>
        ///    Gender 性别<br/>
        ///    Phone 手机号<br/>
        ///    RoleId 用户权限id<br/>
        ///    Role 根据权限id查权限名<br/>
        ///    is_enabled 是否被启用<br/>
        ///     
        ///    
        /// }
        /// </response>
        /// <returns></returns>
        [HttpGet("profile")]
        [Authorize(Roles ="7")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.SysUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(user);
        }

        /// <summary>
        /// 获取用户列表部分逻辑
        /// </summary>
        /// <response code="200">
        /// 返回用户信息
        /// {<br/>
        ///     包含user实体类中的全部内容<br/>
        ///    Id 用户id<br/>
        ///    Username 用户名<br/>
        ///    Password 经hash加密过的密码 可以无视<br/>
        ///    RealName 用户真名<br/>
        ///    Email 电子邮件<br/>
        ///    Gender 性别<br/>
        ///    Phone 手机号<br/>
        ///    RoleId 用户权限id<br/>
        ///    Role 根据权限id查权限名<br/>
        ///    is_enabled 是否被启用<br/>
        ///     
        ///    
        /// }
        /// </response>
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
        /// <response code="200">
        /// 返回用户信息<br/>
        /// {<br/>
        ///     包含user实体类中的全部内容<br/>
        ///    Id 用户id<br/>
        ///    Username 用户名<br/>
        ///    Password 经hash加密过的密码 可以无视<br/>
        ///    RealName 用户真名<br/>
        ///    Email 电子邮件<br/>
        ///    Gender 性别<br/>
        ///    Phone 手机号<br/>
        ///    RoleId 用户权限id<br/>
        ///    Role 根据权限id查权限名<br/>
        ///    is_enabled 是否被启用<br/>
        ///     
        ///    
        /// }
        /// </response>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.SysUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound("用户不存在");
            return Ok(user);
        }

        /// <summary>
        /// 修改用户信息部分逻辑(roleid = 6 7)
        /// </summary>
        /// <response code="404">
        /// 用户不存在
        /// </response>
        ///<response code="200">
        ///修改成功
        /// </response>
        /// <param name="id">用户id</param>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpPut("update/{id}")]
        [Authorize(Roles ="6,7")]
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
        /// 修改密码部分逻辑(roleid = 6 7)
        /// </summary>
        ///<response code="200">
        ///修改成功
        /// </response>
        /// <param name="Dto"></param>
        /// <returns></returns>
        [HttpPut("change-pwd")]
        [Authorize(Roles ="6,7")]
        public async Task<IActionResult> ChangePwd(ChangePwdDto Dto)
        {
            var userId = Dto.UserId;
            var user = await _context.SysUsers.FindAsync(userId);

            if (!BCrypt.Net.BCrypt.Verify(Dto.OldPassword, user.Password))
                return BadRequest("原密码错误");

            user.Password = BCrypt.Net.BCrypt.HashPassword(Dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("密码修改成功");
        }

        /// <summary>
        /// 启用/禁用用户部分逻辑(role = 6)
        /// </summary>
        ///<response code="404">
        ///用户不存在
        /// </response>
        ///<response code="200">
        ///禁用/启用成功
        /// </response>
        /// <param name="id"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        [HttpPut("enable/{id}")]
        [Authorize(Roles = "6")]
        public async Task<IActionResult> EnableUser(int id, [FromQuery] bool isEnabled)
        {

            var user = await _context.SysUsers.FindAsync(id);

            if (user == null) return NotFound("用户不存在");

            user.IsEnabled = isEnabled;
            await _context.SaveChangesAsync();

            return Ok(isEnabled ? "已启用" : "已禁用");
        }

        /// <summary>
        /// 删除用户部分逻辑(roleid = 6)
        /// </summary>
        ///<response code="404">
        ///用户不存在
        /// </response>
        ///<response code="200">
        ///删除成功
        /// </response>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "6")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.SysUsers.FindAsync(id);
            if (user == null) return NotFound("用户不存在");

            _context.SysUsers.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("删除成功");
        }

        /// <summary>
        /// 修改用户权限等级(roleid = 6)
        /// </summary>
        ///<response code="404">
        ///用户不存在
        /// </response>
        ///<response code="200">
        ///已修改权限等和新的权限等级 roleid
        /// </response>
        /// <param name="id">要修改的用户ID</param>
        /// <param name="newRoleId">新的权限角色ID</param>
        /// <returns></returns>
        [HttpPut("change-role/{id}")]
        [Authorize(Roles = "6")]
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
        /// 根据姓名/用户名模糊查询用户(roleid =6)
        /// </summary>
        /// <param name="keyword">搜索关键词（姓名/用户名）</param>
        /// <param name="page">页码</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(Roles = "6")]
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
