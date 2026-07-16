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

        public override TenorMedia? GetMedia(string key)
        {
            if (Media == null || Media.Count == 0) return null;
            if (!Media.FirstOrDefault()!.TryGetValue(key, out var res)) return null;

            return res;
        }
    }
}
