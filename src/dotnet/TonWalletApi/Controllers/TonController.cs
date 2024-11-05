using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TonWalletApi.Services;

namespace TonWalletApi.Controllers
{
    [Route("api/ton")]
    [ApiController]
    [Authorize]
    public class TonController : ControllerBase
    {
        private readonly ITonService _tonService;

        public TonController(ITonService tonService)
        {
            _tonService = tonService;
        }

        // GET: api/ton/jettons
        [HttpGet("jettons")]
        public async Task<IActionResult> GetJettonsAsync()
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                return Forbid("You can only access your own jettons.");
            }

            var jettons = await _tonService.GetJettonsAsync(int.Parse(userIdFromToken));
            return Ok(jettons);
        }

        // GET: api/ton/tonHistory
        [HttpGet("tonHistory")]
        public async Task<IActionResult> GetTonHistoryAsync()
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                return Forbid("You can only access your own TON history.");
            }

            var history = await _tonService.GetTonHistoryAsync(int.Parse(userIdFromToken));
            return Ok(history);
        }

        // GET: api/ton/jettonHistory/{jettonAddress}
        [HttpGet("jettonHistory")]
        public async Task<IActionResult> GetJettonHistoryAsync(string jettonAddress)
        {
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                return Forbid("You can only access your own jetton history.");
            }

            var history = await _tonService.GetJettonHistoryAsync(int.Parse(userIdFromToken), jettonAddress);
            return Ok(history);
        }

        // GET: api/ton/jettonChart/{jettonAddress}?startDate={startDate}
        [HttpGet("jettonChart")]
        public async Task<IActionResult> GetJettonChartAsync(string jettonAddress, long startDate)
        {
            var chart = await _tonService.GetJettonChartAsync(jettonAddress, startDate);
            return Ok(chart);
        }
    }
}