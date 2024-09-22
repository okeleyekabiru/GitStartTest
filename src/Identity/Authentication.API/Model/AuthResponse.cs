namespace Authentication.API.Model
{
    public class AuthResponse
    {
        public string? AuthToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}