
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TonWalletBot.Commands
{
    public class StartCommand : BaseCommand
    {
        private readonly ITelegramBotClient _botClient;

        public StartCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(Update update)
        {

            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Welcome back! Your character is ready.");
            return;
        }
    }
}
