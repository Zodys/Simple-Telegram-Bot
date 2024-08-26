using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using System.Data;
using System.Text.RegularExpressions;
using telebot.Commands;

namespace telebot
{
    internal class Program
    {
        static async void OnMessage(ITelegramBotClient client, Update update)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        var message = update.Message;
                        string messageText = await HandleCommand(client, message);
                        if (messageText != null)
                            await client.SendTextMessageAsync(message.Chat.Id, messageText);
                        break;

                    case UpdateType.CallbackQuery:
                        var callbackQuery = update.CallbackQuery;
                        string callBackText = await HandleCallback(callbackQuery.Data);
                        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, callBackText);
                        await client.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                        break;
                    case UpdateType.InlineQuery:
                        var inlineQuery = update.InlineQuery;
                        await OnInlineQueryReceived(client, inlineQuery!);
                        break;
                    case UpdateType.ChosenInlineResult:
                        var chosenInlineResult = update.ChosenInlineResult;
                        await OnChosenInlineResultReceived(client, chosenInlineResult!);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.ToString());
            }
        }

        private readonly static Dictionary<string, string> inlineCommands = new Dictionary<string, string>
        {
            { "Random", "Генерирует случайное число в диапазоне (1, 100), можно ввести свой диапазон чисел" },
            { "Time", "Показывает текущее время" }
        };

        static async Task OnChosenInlineResultReceived(ITelegramBotClient bot, ChosenInlineResult chosenInlineResult)
        {
            if (uint.TryParse(chosenInlineResult.ResultId, out var resultId)
                && resultId < inlineCommands.Count)
            {
                var command = inlineCommands.ElementAt((int)resultId).Key;
                
            }

            await Task.CompletedTask;
        }


        static async Task OnInlineQueryReceived(ITelegramBotClient bot, InlineQuery inlineQuery)
        {
            var results = new List<InlineQueryResult>();

            var counter = 0;
            foreach (var (command, description) in inlineCommands)
            {
                var article = new InlineQueryResultArticle(
                    $"{counter}",
                    command,
                    new InputTextMessageContent(await HandleInlineCallback(command, inlineQuery))
                );
                article.Description = description;
                results.Add(article);
                counter++;
            }
            await bot.AnswerInlineQueryAsync(inlineQuery.Id, results, cacheTime: 5, isPersonal: true);
        }

        static async Task<string> HandleInlineCallback(string callbackData, InlineQuery inlineQuery)
        {
            var handler = new CommandHandler();
            var args = inlineQuery.Query.Split(' ');
            return handler.Handle(callbackData, args);
            
        }

        static async Task<string> HandleCommand(ITelegramBotClient client, Message message)
        {
            var command = message.Text?.ToString();
            var commandParts = command.Split(' ');
            var handler = new CommandHandler();

            Logger.Log(LogLevel.Info, $"{message.Text?.ToString()} {command} ");

            return handler.Handle(commandParts[0].ToLower().Replace("/",""),commandParts.Skip(1).ToArray(), client,message.Chat.Id);
        }

        static async Task<string> HandleCallback(string callbackData)
        {
            var handler = new CommandHandler();
            return handler.Handle(callbackData);
        }

        private static void Main(string[] args)
        {

            Host botClient = new Host("token");

            botClient.OnMessage += OnMessage;
            botClient.Start();
            Thread.Sleep(-1);
        }

    }
}