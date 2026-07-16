using System.Text.Json.Serialization;

namespace TGTenorSearch.Models
{
    public class TenorResult
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("media")]
        public List<Dictionary<string, TenorMedia>>? Media { get; set; }
    }
}
