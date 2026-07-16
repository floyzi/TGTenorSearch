using System.Text.Json.Serialization;

namespace TGTenorSearch.Models.Tenor
{
    public abstract class TenorResultBase
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        public abstract TenorMedia? GetMedia(string key);
    }
}
