using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TonWalletApi.Dtos;
using TonWalletApi.Services;

namespace TonWalletApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITonService _tonService;
        public UsersController(IAuthService authService, ITonService tonService) 
        {
            _authService = authService;
            _tonService = tonService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserDto userDto)
        {
            await _authService.LoginAsync(userDto);

            return Ok("Logined successfully");
        }

        [HttpGet("exists")]
        public async Task<IActionResult> IsIserExistAsync(int id)
        {
            var exists = await _authService.IsUserExist(id);
            return Ok(exists);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var user = await _authService.GetUserAsync(id);
            return Ok(user);
        }

        [HttpGet("jettons")]
        public async Task<IActionResult> GetJettonstAsync(int userId)
        {
            var user = await _tonService.GetJettonsAsync(userId);
            return Ok(user);
        }

        [HttpGet("tonHistory")]
        public async Task<IActionResult> GetTonHistoryAsync(int userId)
        {
            var history = await _tonService.GetTonHistoryAsync(userId);
            return Ok(history);
        }

        [HttpGet("jettonHistory")]
        public async Task<IActionResult> GetJettonHistoryAsync(int userId, string jettonAddress)
        {
            var history = await _tonService.GetJettonHistoryAsync(userId, jettonAddress);
            return Ok(history);
        }

        [HttpGet("jettonChart")]
        public async Task<IActionResult> GetJettonChartAsync(string jettonAddress, long startDate)
        {
            var chart = await _tonService.GetJettonChartAsync(jettonAddress, startDate);
            return Ok(chart);
        }
    }
}
