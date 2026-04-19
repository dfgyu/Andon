namespace Andon.Dtos
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RealName { get; set; }
        public int? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int RoleId { get; set; } = 0;1
    }
}
