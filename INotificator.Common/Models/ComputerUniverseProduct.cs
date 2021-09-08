using System;
using System.Text.Json.Serialization;

namespace INotificator.Common.Models
{
    public class ComputerUniverseProduct
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("priceRur")]
        public string PriceRur { get; set; }
        [JsonPropertyName("priceEur")]
        public string PriceEur { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}