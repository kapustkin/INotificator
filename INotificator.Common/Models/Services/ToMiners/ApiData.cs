using System;
using System.Text.Json.Serialization;

namespace INotificator.Common.Models.Services.ToMiners
{
    public class ApiData
    {
        [JsonPropertyName("apiVersion")]
        public int ApiVersion { get; set; }
        
        [JsonPropertyName("workersOnline")]
        public int WorkersOnline { get; set; }
        
        [JsonPropertyName("hashrate")]
        public double Hashrate { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public double UpdatedAt { get; set; }
        
        [JsonPropertyName("paymentsTotal")]
        public int PaymentsTotal { get; set; }
        
        [JsonPropertyName("payments")]
        public Payment[] Payments{ get; set; }
    }
}