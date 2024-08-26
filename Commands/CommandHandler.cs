namespace telebot.Commands
{
    public class CommandHandler
    {
        private readonly Dictionary<string, ICommand> commands;

        public CommandHandler()
        {
            commands = new Dictionary<string, ICommand>
            {
                { "start", new StartCommand()},
                { "time", new TimeCommand() },
                { "help", new HelpCommand() },
                { "random", new RandomCommand() }
            };
        }

        public string Handle(string commandKey, params object[] args)
        {
            ICommand command;
            if (commands.TryGetValue(commandKey.ToLower(), out command))
            {
                return command.Execute(args);
            }
            else
            {
                return "Неизвестный запрос.";
            }
        }

        public static string[] GetValues(object[] args)
        {
            if (args.Length > 0 && args[0] is string[] && args is not string[])
            {
                return (string[])args[0];
            }
            else if(args.Length > 0 && args is string[])
            {
                return (string[])args;
            }
            else{
                return Array.Empty<string>();
            }
        }

    }

}