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

            return GetPreferredMedia(medias);
        }
    }
}
