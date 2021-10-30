using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Models;

namespace INotificator.Services.Parser
{
    public class BasicApiParser: IBasicApiParser
    {
        public class DateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Debug.Assert(typeToConvert == typeof(DateTime));

                if (reader.TokenType == JsonTokenType.Number)
                {
                    // Unix time stamp
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dateTime = dateTime.AddSeconds(reader.GetDouble()).ToLocalTime();
                    return dateTime;
                }

                return DateTime.ParseExact(reader.GetString(), "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        
        public DataResult<T> ParseResult<T>(string data)
        {
            var result = new DataResult<T>()
            {
                Data = JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions()
                {
                    Converters =
                    {
                        new DateTimeConverter()
                    }
                })
            };
            
            return result;
        }
    }
}