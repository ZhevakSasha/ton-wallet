using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TonWalletApi.Dtos;
using TonWalletApi.Services;

namespace TonWalletApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserDto userDto)
        {
            var token = await _authService.LoginAsync(userDto);
            return Ok(token);
        }

        // POST: api/users/logout
        [HttpPost("{id:int}/logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync(int id)
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken != id.ToString())
            {
                return Forbid("You can only log out your own account.");
            }

            await _authService.LogoutAsync(id);
            return Ok(new { message = "Logged out successfully" });
        }

        // GET: api/users/exists/{id}
        [HttpGet("exists/{id:int}")]
        public async Task<IActionResult> IsUserExistAsync(int id)
        {
            var exists = await _authService.IsUserExist(id);
            return Ok(new { exists });
        }

        // POST: api/users/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenDto tokenDto)
        {
            var authResult = await _authService.RefreshTokenAsync(tokenDto);
            if (authResult == null)
            {
                return Unauthorized(new { message = "Invalid refresh token." });
            }
            return Ok(authResult);
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken != id.ToString())
            {
                return Forbid("You can only access your own profile.");
            }

            var user = await _authService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
    }
}
