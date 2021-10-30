using System;
using System.Text.Json.Serialization;

namespace INotificator.Common.Models.Services.Hpool
{
    public class LogRecord
    {
        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("level")]
        public string Level { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("capacity")]
        public string Capacity { get; set; }
        [JsonPropertyName("errorDescription")]
        public string ErrorDescription { get; set; }
    }
}