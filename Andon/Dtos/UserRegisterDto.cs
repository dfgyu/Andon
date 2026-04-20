namespace Andon.Dtos
{
    public class UserRegisterDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>                                      
        public string Password { get; set; }
        /// <summary>
        /// 真实名字
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? Gender { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>                  
        public string? Email { get; set; }
        /// <summary>
        /// 权限ID 默认为3
        /// </summary>
        public int RoleId { get; set; } = 3;
    }
}
