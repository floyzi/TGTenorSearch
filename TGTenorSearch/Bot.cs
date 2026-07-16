using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TGTenorSearch.Models;

namespace TGTenorSearch
{
    internal class Bot
    {
        TelegramBotClient? bot;

        internal async Task Launch(string token)
        {
            bot = new TelegramBotClient(token);

            var me = await bot!.GetMe();

            Console.WriteLine("connected to " + me.Username);

            await bot!.DropPendingUpdates();

            bot.OnUpdate += OnUpdate;
        }

        async Task OnUpdate(Update update)
        {
            Console.WriteLine(update.Type);
            switch (update.Type)
            {
                case UpdateType.InlineQuery:
                    await OnInlineQueryReceived(update.InlineQuery);
                    break;
            }
        }

        
        async Task OnInlineQueryReceived(InlineQuery inlineQuery)
        {
            var results = new List<InlineQueryResult>();

            using var client = new HttpClient();

            var uri = new UriBuilder("https://api.tenor.com/v1/search");

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["key"] = "3Z0688EVWYKH";
            query["locale"] = "en_us";
            query["q"] = inlineQuery.Query;
            query["limit"] = "50";

            if (!string.IsNullOrEmpty(inlineQuery.Offset))
                query["pos"] = inlineQuery.Offset;

            uri.Query = query.ToString();

            var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri.Uri));

            var json = JsonSerializer.Deserialize<TenorResponse>(await res.Content.ReadAsStringAsync());

            foreach (var gif in json.Results)
            {
                var gifMedia = gif.Media[0]["gif"];

                Console.WriteLine(gif.Media[0]["gif"]);

                results.Add(new InlineQueryResultGif()
                {
                    Id = gif.Id,
                    GifUrl = gifMedia.Url,
                    ThumbnailUrl = gifMedia.Preview,
                 
                    GifWidth = gifMedia.Dimensions.ElementAt(0),
                    GifHeight = gifMedia.Dimensions.ElementAt(1),
                    
                });
            }

            await bot.AnswerInlineQuery(inlineQuery.Id, results, nextOffset: json.Next, cacheTime: 0);
        }
    }
}
