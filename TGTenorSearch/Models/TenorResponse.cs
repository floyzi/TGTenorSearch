using System.Text.Json.Serialization;

namespace TGTenorSearch.Models
{
    public class TenorResponse
    {
        [JsonPropertyName("results")]
        public List<TenorResult>? Results { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }
    }
}
