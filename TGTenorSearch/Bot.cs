using System.Diagnostics;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TGTenorSearch.Models.Tenor.V1;
using TGTenorSearch.Models.Tenor.V2;

namespace TGTenorSearch
{
    internal class Bot
    {
        string? currentUsername;
        TelegramBotClient? bot;
        TenorRequests? requestSender;

        internal async Task Launch(string token)
        {
            requestSender = new();

            Console.WriteLine("Connected to Telegram bot API...");

            bot = new(token);

            var me = await bot!.GetMe();

            currentUsername = me.Username;
            Console.WriteLine("Connected to @" + currentUsername);

            await bot!.DropPendingUpdates();

            bot.OnUpdate += OnUpdate;
        }

        async Task OnUpdate(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await OnMessageReceived(update.Message!);
                    break;
                case UpdateType.InlineQuery:
                    await OnInlineQueryReceived(update.InlineQuery!);
                    break;
            }
        }

        async Task OnMessageReceived(Message message)
        {
            if (bot == null) throw new InvalidOperationException();

            //days without proper telegram bot command handling: 4719
            if ((message.Chat.Type == ChatType.Private && message.Text == "/start") || message.Text == "/start@" + currentUsername)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"This bot can help you find and share GIFs. " +
                    $"It works automatically, no need to add it anywhere. " +
                    $"Simply open any of your chats and type <code>@{currentUsername} something</code> in the message field. " +
                    $"Then tap on a result to send.\n\n" +
                    $"For example, try typing <code>@{currentUsername} happy dog</code> here.");

                if (message.From?.Id == Program.Config!.DevID)
                {
                    var upT = DateTime.UtcNow - Program.StartedAt;
                    var mem = Process.GetCurrentProcess().PrivateMemorySize64 / 1024.0 / 1024.0;

                    sb.AppendLine($"\nDEBUG: uptime: {(int)upT.TotalDays}d {upT.Hours}h {upT.Minutes}m {upT.Seconds}s | mem: {mem:F1} MB | commit: {TGTenorSearchBuildDetails.CommitHash[..12]}");
                }

                await bot.SendMessage(message.Chat, sb.ToString(), ParseMode.Html);
            }
        }

        async Task OnInlineQueryReceived(InlineQuery inlineQuery)
        {
            if (bot == null || requestSender == null) throw new InvalidOperationException();

            try
            {
                Console.WriteLine(inlineQuery.From.LanguageCode);

                if (string.IsNullOrEmpty(Program.Config!.APIKey)) throw new InvalidOperationException("No tenor API key provided");

                var queryResult = Program.IsV1
                    ? await requestSender.GetResults<TenorResponseV1>(inlineQuery.Query, inlineQuery.Offset, inlineQuery.From.LanguageCode) 
                    : await requestSender.GetResults<TenorResponseV2>(inlineQuery.Query, inlineQuery.Offset, inlineQuery.From.LanguageCode);

                await bot.AnswerInlineQuery(inlineQuery.Id, queryResult.Item1, 0, false, queryResult.Item2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await bot.AnswerInlineQuery(inlineQuery.Id,
                [
                    new InlineQueryResultArticle()
                    {
                        Id = inlineQuery.Id,
                        Title = "Error",
                        Description = "An exception was thrown, click to see the details",
                        InputMessageContent = new InputTextMessageContent(e.ToString())
                    }
                ], 0);
            }
        }
    }
}
