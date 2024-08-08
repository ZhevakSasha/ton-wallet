using Telegram.Bot.Types;

namespace TonWalletBot.Services
{
    public interface ICommandExecutor
    {
        Task Execute(Update update);
    }
}
