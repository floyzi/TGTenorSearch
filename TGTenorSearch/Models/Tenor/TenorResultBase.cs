using System.Text.Json.Serialization;

namespace TGTenorSearch.Models.Tenor
{
    public abstract class TenorResultBase
    {
        protected const long MAX_MEDIA_SIZE = 10486760;

        [JsonPropertyName("id")]
        public required string Id { get; set; }
        public abstract TenorMedia? GetMedia();
    }
}
