namespace Authentication.API.Model
{
    public class RegisterUserDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginUserDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}