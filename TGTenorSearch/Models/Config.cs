using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TGTenorSearch.Models
{
    public class Config
    {
        [JsonPropertyName("token")]
        public string? BotToken { get; set; }

        [JsonPropertyName("key")]
        public string? APIKey { get; set; }

        [JsonPropertyName("client_key")]
        public string? ClientKey { get; set; }

        [JsonPropertyName("dev_id")]
        public long DevID { get; set; }
    }
}
