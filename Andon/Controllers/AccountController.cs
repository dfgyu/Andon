using Andon.Dtos;
using Andon.Models;
using Andon.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace Andon.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AccountController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //注册部分逻辑
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
                RoleId = Dto.RoleId
            };
            _context.SysUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { msg = "注册成功", userId = user.Id });
        }
        //用户登录部分逻辑
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

     }
}
