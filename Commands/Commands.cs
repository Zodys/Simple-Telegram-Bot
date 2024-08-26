
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace telebot.Commands
{
    public class TimeCommand : ICommand
    {
        public string Execute(params object[] args)
        {
            return DateTime.Now.ToString();
        }
    }

    public class HelpCommand : ICommand
    {
        private readonly Dictionary<string, string> commands = new Dictionary<string, string>
        {
            { "/start", "Начать работу с ботом" },
            { "/time", "Текущее время" },
            { "/help", "Справка. Можно получить дополнительную информацию о командах, пример использования /help random" },
            { "/random", "Сгенерировать случайное число от 1 до 100" }
        };

        private readonly Dictionary<string, string> commandHelpInfo = new Dictionary<string, string>
        {
            {"/random", "Возвращяет случайное число в диапазоне от 1 до 100, можно ввести свой диапазон. Пример: /random 1 5"}
        };

        public string Execute(params object[] args)
        {
            var sb = new StringBuilder();
            string[] values = CommandHandler.GetValues(args);
            if (values.Length > 0 && values[0] is string command)
            {
                if (!command.StartsWith('/'))
                {
                    command = '/' + command;
                }
                if (commands.TryGetValue(command, out var description))
                {
                    sb.AppendLine($"{command} - {description}");

                    if (commandHelpInfo.TryGetValue(command, out var helpInfo))
                    {
                        sb.AppendLine($"Дополнительная информация: {helpInfo}");
                    }
                }
                else
                {
                    sb.AppendLine($"Неизвестная команда {command}.");
                }
            }
            else
            {
                sb.AppendLine("Доступные команды:");

                foreach (var (comm, description) in commands)
                {
                    sb.AppendLine($"{comm} - {description}");
                }
            }

            return sb.ToString();
        }
    }


    public class RandomCommand : ICommand
    {
        public string Execute(params object[] args)
        {
            int min = 1;
            int max = 100;

            string[] values = CommandHandler.GetValues(args);
            if (values != null && values.Length == 2)
            {
                if (!int.TryParse(values[0], out min) || !int.TryParse(values[1], out max))
                {
                    return "Аргументы должны быть целыми числами";
                }
            }

            Random random = new Random();
            int randomNumber = random.Next(Math.Min(min, max), Math.Max(min, max) + 1);
            return $"Случайное число: {randomNumber}";
        }
    }

    public class StartCommand : ICommand
    {
        public string Execute(params object[] args)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Время", "time"),
                    InlineKeyboardButton.WithCallbackData("Справка", "help"),
                    InlineKeyboardButton.WithCallbackData("Случайное число", "random")
                }
            });
            ITelegramBotClient client = (ITelegramBotClient)args[1];
            long chatId = (long)args[2];
            client.SendTextMessageAsync(chatId, "Выберите действие:", replyMarkup: keyboard);

            return null;
        }
    }


}