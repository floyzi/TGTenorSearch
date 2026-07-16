using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Web;
using Telegram.Bot.Types.InlineQueryResults;
using TGTenorSearch.Models.Tenor;

namespace TGTenorSearch
{
    internal class TenorRequests
    {
        const string TENOR_V1_API = "https://api.tenor.com/v1";
        const string TENOR_V2_API = "https://tenor.googleapis.com/v2";

        HttpClient? client = new();

        internal async Task<(List<InlineQueryResult>, string)> GetResults<TTenorResponse, TTenorResult>(string q, bool isV1, string? offset = "")
          where TTenorResponse : TenorResponseBase<TTenorResult>
          where TTenorResult : TenorResultBase
        {
            var results = new List<InlineQueryResult>();

            var tenorResult = await Search<TTenorResponse>(q, isV1, offset);

            if (tenorResult == null || tenorResult.Results == null || tenorResult.Results.Count == 0) return new(results, "");

            foreach (var gif in tenorResult.Results!)
            {
                if (string.IsNullOrEmpty(gif.Id)) continue;

                var gifMedia = gif.GetMedia();

                if (gifMedia == null || string.IsNullOrEmpty(gifMedia.Url)) continue;

                var gifRes = new InlineQueryResultGif()
                {
                    Id = gif.Id,
                    GifUrl = gifMedia.Url,
                    ThumbnailUrl = !string.IsNullOrEmpty(gifMedia.Preview) ? gifMedia.Preview : gifMedia.Url,
                    GifDuration = (int)gifMedia.Duration,
                };

                if (gifMedia.Dimensions != null && gifMedia.Dimensions.Length == 2)
                {
                    gifRes.GifWidth = gifMedia.Dimensions[0];
                    gifRes.GifHeight = gifMedia.Dimensions[1];
                }

                results.Add(gifRes);
            }

            return new(results, tenorResult.Next!);
        }

        async Task<T> Search<T>(string q, bool isV1, string? offset = "") where T : class
        {
            if (client == null) throw new InvalidOperationException();

            if (!isV1 && string.IsNullOrEmpty(Program.Config!.ClientKey)) throw new InvalidOperationException("No client key provided for V2 api request");

            var uri = new UriBuilder(isV1
                ? (TENOR_V1_API + (!string.IsNullOrEmpty(q) ? "/search" : "/trending"))
                : TENOR_V2_API + (!string.IsNullOrEmpty(q) ? "/search" : "/featured"));

            var query = HttpUtility.ParseQueryString(string.Empty);

            query["key"] = Program.Config!.APIKey;
            query["locale"] = "en_US";

            if (!string.IsNullOrEmpty(q))
                query["q"] = q;

            query["limit"] = "50";
            query["media_filter"] = isV1 ? "minimal" : "gif,tinygif,mp4";

            if (!isV1)
                query["client_key"] = Program.Config.ClientKey;

            if (!string.IsNullOrEmpty(offset))
                query["pos"] = offset;

            uri.Query = query.ToString();

            var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri.Uri));

            res.EnsureSuccessStatusCode();

            var str = await res.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(str)) throw new Exception("Invalid tenor response");

            return JsonSerializer.Deserialize<T>(str)!;
        }
    }
}
