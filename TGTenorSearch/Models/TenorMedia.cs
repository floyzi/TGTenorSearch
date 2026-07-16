using System.Text.Json.Serialization;

namespace TGTenorSearch.Models
{
    public class TenorMedia
    {
        [JsonPropertyName("preview")]
        public required string Preview { get; set; }

        [JsonPropertyName("url")]
        public required string Url { get; set; }

        [JsonPropertyName("duration")]
        public float Duration { get; set; }

        [JsonPropertyName("dims")]
        public int[]? Dimensions { get; set; }
    }
}
