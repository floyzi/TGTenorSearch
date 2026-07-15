using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

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

            var json = JsonElement.Parse(await res.Content.ReadAsStringAsync());

            foreach (var gif in json.GetProperty("results").EnumerateArray())
            {
                var gifMedia = gif.GetProperty("media").EnumerateArray().FirstOrDefault().GetProperty("gif");

                var gifDims = gifMedia.GetProperty("dims").EnumerateArray();

                results.Add(new InlineQueryResultGif()
                {
                    Id = gif.GetProperty("id").GetString(),
                    GifUrl = gifMedia.GetProperty("url").GetString(),
                    ThumbnailUrl = gifMedia.GetProperty("preview").GetString(),
                    GifDuration = (int)gifMedia.GetProperty("duration").GetSingle(),
                    GifWidth = gifDims.ElementAt(0).GetInt32(),
                    GifHeight = gifDims.ElementAt(1).GetInt32(),
                    
                });
            }

            await bot.AnswerInlineQuery(inlineQuery.Id, results, nextOffset: json.GetProperty("next").GetString(), cacheTime: 0);
        }
    }
}
