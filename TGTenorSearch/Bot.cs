using System.Text.Json;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TGTenorSearch.Models;

namespace TGTenorSearch
{
    internal class Bot
    {
        const string TENOR_API = "https://api.tenor.com/v1/";
        string? currentUsername;
        TelegramBotClient? bot;
        HttpClient? client;

        internal async Task Launch(string token)
        {
            client = new();
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

            if (message.Text == "/start" || message.Text == "/start@" + currentUsername) //days without proper telegram bot commands handling: 4719
            {
                await bot.SendMessage(message.Chat, $"This bot can help you find and share GIFs. " +
                    $"It works automatically, no need to add it anywhere. " +
                    $"Simply open any of your chats and type <code>@{currentUsername} something</code> in the message field. " +
                    $"Then tap on a result to send.\n\n" +
                    $"For example, try typing <code>@{currentUsername} happy dog</code> here.", ParseMode.Html);
            }
        }

        
        async Task OnInlineQueryReceived(InlineQuery inlineQuery)
        {
            if (bot == null) throw new InvalidOperationException();

            var results = new List<InlineQueryResult>();

            try
            {
                var tenorResult = await Search(inlineQuery.Query, inlineQuery.Offset);

                if (tenorResult.Results == null || tenorResult.Results.Count == 0)
                {
                    await bot.AnswerInlineQuery(inlineQuery.Id, results);
                    return;
                }

                foreach (var gif in tenorResult.Results)
                {
                    if (gif.Media == null || gif.Media.Count == 0) continue;
                    if (!gif.Media.FirstOrDefault()!.TryGetValue("gif", out var gifMedia)) continue;

                    if (string.IsNullOrEmpty(gif.Id) || string.IsNullOrEmpty(gifMedia.Url) || string.IsNullOrEmpty(gifMedia.Preview)) continue;

                    var gifRes = new InlineQueryResultGif()
                    {
                        Id = gif.Id,
                        GifUrl = gifMedia.Url,
                        ThumbnailUrl = gifMedia.Preview,
                        GifDuration = (int)gifMedia.Duration,
                    };

                    if (gifMedia.Dimensions != null && gifMedia.Dimensions.Length == 2)
                    {
                        gifRes.GifWidth = gifMedia.Dimensions[0];
                        gifRes.GifHeight = gifMedia.Dimensions[1];
                    }

                    results.Add(gifRes);
                }

                await bot.AnswerInlineQuery(inlineQuery.Id, results, 0, false, tenorResult.Next);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await bot.AnswerInlineQuery(inlineQuery.Id, results);
            }
        }

        async Task<TenorResponse> Search(string q, string? offset = "")
        {
            if (client == null) throw new InvalidOperationException();

            var uri = new UriBuilder(TENOR_API + (!string.IsNullOrEmpty(q) ? "search" : "trending"));

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (string.IsNullOrEmpty(Program.Config!.TenorKey)) throw new InvalidOperationException("No tenor API key provided");

            query["key"] = Program.Config!.TenorKey;
            query["locale"] = "en_US";

            if (!string.IsNullOrEmpty(q))
                query["q"] = q;

            query["limit"] = "50";
            query["media_filter"] = "minimal";

            if (!string.IsNullOrEmpty(offset))
                query["pos"] = offset;

            uri.Query = query.ToString();

            var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri.Uri));

            res.EnsureSuccessStatusCode();

            var str = await res.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(str)) throw new Exception("Invalid response");

            return JsonSerializer.Deserialize<TenorResponse>(str)!;
        }
    }
}
