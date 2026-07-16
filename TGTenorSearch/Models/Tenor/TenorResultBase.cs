using System.Text.Json.Serialization;

namespace TGTenorSearch.Models.Tenor
{
    public abstract class TenorResultBase
    {
        protected const long MAX_MEDIA_SIZE = 10486760;

        [JsonPropertyName("id")]
        public required string Id { get; set; }
        public abstract TenorMedia? GetMedia();
        protected static TenorMedia GetPreferredMedia(Dictionary<string, TenorMedia> media)
        {
            foreach (var mediaType in new string[] { "gif", "mp4", "tinygif" })
            {
                if (media.TryGetValue(mediaType, out var result) && result.Size < MAX_MEDIA_SIZE)
                    return result;
            }

            return null!;
        }
    }
}
