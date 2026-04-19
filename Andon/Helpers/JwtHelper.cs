using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Andon.Models;

namespace Andon.Helpers
{
    public class JwtHelper
    {
        //  静态方法 + 完全使用传入的 config，不依赖类的实例变量
        public static string GenerateToken(SysUser user, IConfiguration config)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.RealName),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim("RoleName", user.Role!.RoleName!)
            };

            //  全部用传入的 config，不要用 _config
            var secretKey = config["Jwt:Secret"];
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];
            var expireMinutes = Convert.ToDouble(config["Jwt:ExpireMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}