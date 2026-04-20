namespace Andon.Dtos
{
    public class UserUpdateDto
    {
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>   
        /// 性别
        /// </summary>
        public int Gender { get; set; }
        /// <summary>   
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        public int RoleId { get; set; }
    }
}
