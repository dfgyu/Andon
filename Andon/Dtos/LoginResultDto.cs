namespace Andon.Dtos
{
    public class LoginResultDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Token { get; set; }
    }
}
