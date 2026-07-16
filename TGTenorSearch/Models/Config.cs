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
    }
}
