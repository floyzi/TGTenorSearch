using System.Text.Json.Serialization;
using TGTenorSearch.Models.Tenor.V1;

namespace TGTenorSearch.Models.Tenor
{
    public abstract class TenorResponseBase<TResult> : ITenorResponse where TResult : TenorResultBase
    {
        [JsonPropertyName("results")]
        public List<TResult>? Results { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        IEnumerable<TenorResultBase>? ITenorResponse.Results => Results;
    }
}
