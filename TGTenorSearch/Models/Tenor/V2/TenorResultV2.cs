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

        public override TenorMedia? GetMedia(string key)
        {
            if (Media == null || Media.Count == 0) return null;
            if (!Media.TryGetValue(key, out var res)) return null;

            return res;
        }
    }
}
