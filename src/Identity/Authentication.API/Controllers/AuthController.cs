using Authentication.API.Model;
using Authentication.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService userService, ITokenService tokenService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var response = await userService.RegisterUserAsync(registerDto.Username, registerDto.Email, registerDto.Password);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var token = await userService.LoginUserAsync(loginDto.Email, loginDto.Password);
            return Ok(token);
        }
    }
}