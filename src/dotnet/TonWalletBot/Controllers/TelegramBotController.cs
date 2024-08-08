using TonWalletBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TonWalletBot.Controllers
{
    [Route("bot")]
    [ApiController]
    public class TelegramBotController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        public TelegramBotController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
           if (update.Message?.Chat == null && update.CallbackQuery == null)
            {
                return Ok();
            }

            try
            {
                await _commandExecutor.Execute(update);
            }
            catch (Exception e)
            {
                return Ok();
            }

            return Ok();
        }
    }
}
