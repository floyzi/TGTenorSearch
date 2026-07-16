using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TGTenorSearch.Models.Tenor.V1
{
    internal class TenorResultV1 : TenorResultBase
    {
        [JsonPropertyName("media")]
        public List<Dictionary<string, TenorMedia>>? Media { get; set; }

        public override TenorMedia? GetMedia()
        {
            if (Media == null || Media.Count == 0) return null;

            var medias = Media.FirstOrDefault();
            if (medias == null) return null;
            foreach (var media in new string[] { "gif", "mp4", "tinygif" })
            {
                if (medias.TryGetValue(media, out var result) && result.Size < MAX_MEDIA_SIZE)
                    return result;
            }

            return null;
        }
    }
}
