using Authentication.API.Model;
using Authentication.API.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid userId)
        {
            var response = await userService.DeleteUserAsync(userId);
            return Ok(response);
        }

        [HttpPut("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> Put([FromRoute] Guid userId, [FromBody] RegisterUserDto registerDto)
        {
            var response = await userService.UpdateUserAsync(userId, registerDto.Username, registerDto.Email, registerDto.Password);
            return Ok(response);
        }
    }
}