using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using TonWalletBot.Services;
using TonWalletBot.Commands;

namespace TonWalletBot.Services
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly List<BaseCommand> _commands;
        private BaseCommand _command;

        public CommandExecutor(IServiceProvider serviceProvider)
        {
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
        }

        public async Task Execute(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            if (update.Message != null && update.Message.Text.Contains(CommandNames.StartCommand))
            {
                await ExecuteCommand(CommandNames.StartCommand, update);
                return;
            }
        }

        private async Task ExecuteCommand(string commandName, Update update)
        {
            _command = _commands.First(x => x.Name == commandName);
            
            await _command.ExecuteAsync(update);
        }
    }
}
