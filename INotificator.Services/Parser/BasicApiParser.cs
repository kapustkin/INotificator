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
                return DateTime.ParseExact(reader.GetString(), "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        
        public DataResult<IEnumerable<T>> ParseResult<T>(string data)
        {
            var result = new DataResult<IEnumerable<T>>()
            {
                Data = JsonSerializer.Deserialize<IEnumerable<T>>(data, new JsonSerializerOptions()
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