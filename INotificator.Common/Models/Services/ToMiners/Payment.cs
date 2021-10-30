using System;
using System.Text.Json.Serialization;

namespace INotificator.Common.Models.Services.ToMiners
{
    public class Payment
    {
        [JsonPropertyName("amount")]
        public double Amount { get; set; }
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}