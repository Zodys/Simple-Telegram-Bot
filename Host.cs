using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telebot
{
    public class Host
    {
        public Action<ITelegramBotClient,Update>? OnMessage;
        private static TelegramBotClient _bot;
        private static ReceiverOptions _receiverOptions;

        public Host(string token){

            _bot = new TelegramBotClient(token);
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery,
                    UpdateType.InlineQuery,
                    UpdateType.ChosenInlineResult
                },
                ThrowPendingUpdates = true, 
            };
        
        }
        

        public void Start(){

            using var cts = new CancellationTokenSource();
            _bot.StartReceiving(UpdateHandler,ErrorHandler,_receiverOptions,cts.Token);

            Logger.Log(LogLevel.Info,"Бот запущен!");
        }

        private async Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Logger.Log(LogLevel.Error,ErrorMessage);
            await Task.CompletedTask;
        }
        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                OnMessage?.Invoke(botClient,update);
                switch (update.Type)
                {
                    case UpdateType.Message:
                    {
                        var message = update.Message;
                        var user = message.From;
                        Logger.Log(LogLevel.Info,$"{user.FirstName} ({user.Id}) написал сообщение: {message.Text ?? message.Type.ToString()}");
                        return;
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error,ex.ToString());
            }
            await Task.CompletedTask;
        }

    }
}