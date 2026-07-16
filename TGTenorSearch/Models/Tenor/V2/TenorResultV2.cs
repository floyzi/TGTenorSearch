using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TGTenorSearch.Models.Tenor.V2
{
    internal class TenorResultV2 : TenorResultBase
    {
        [JsonPropertyName("media_formats")]
        public Dictionary<string, TenorMedia>? Media { get; set; }

        public override TenorMedia? GetMedia()
        {
            if (Media == null || Media.Count == 0) return null;

            foreach (var media in new string[] { "gif", "mp4", "tinygif" })
            {
                if (Media.TryGetValue(media, out var result) && result.Size < MAX_MEDIA_SIZE) 
                    return result;
            }

            return null;
        }
    }
}
