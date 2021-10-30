using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Models;
using INotificator.Common.Models.Services.BaseProductService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Services.Parser
{
    public class AvitoParser : IAvitoParser
    {
        private readonly ILogger<AvitoParser> _logger;

        private readonly string _url;

        public AvitoParser(
            IOptions<Options> options,
            ILogger<AvitoParser> logger)
        {
            _logger = logger;
            _url = options?.Value?.Avito?.Url ?? throw new ArgumentNullException(nameof(options.Value.Avito.Url));
        }

        public DataResult<ICollection<Product>> ParseResult(string rawData)
        {
            var results = Regex.Matches(rawData, "<div class=\"iva-item-titleStep(.*?)title=\"Добавить в избранное");
            if (!results.Any())
            {
                _logger.LogWarning($"Products not found...");
                return new DataResult<ICollection<Product>>();
            }


            try
            {
                var result = new List<Product>();
                foreach (Match item in results)
                {
                    var url = Regex.Matches(item.Value, "href=\"(.*?)\" target=\"_blank\"").FirstOrDefault();
                    var productName = Regex.Matches(item.Value, "title=\"(.*?)\" ").FirstOrDefault();
                    var price = Regex.Matches(item.Value, "itemProp=\"price\" content=\"(.*?)\"\\/>").FirstOrDefault();

                    if (url != null && productName != null)
                    {
                        result.Add(new Product
                        {
                            Source = _url,
                            Date = DateTime.Now,
                            Name = productName.Groups[1].Value,
                            Url = $"{_url}{url.Groups[1].Value.Replace("amp;dst=rsu", "dst=rsu")}",
                            Price = price.Groups[1].Value
                        });
                        _logger.LogTrace($"Find item: {productName.Groups[1].Value}");
                    }
                    else
                    {
                        _logger.LogWarning($"Unexpected values when parsing {item.Value}");
                    }
                }

                return new DataResult<ICollection<Product>>()
                {
                    Data = result
                };
            }
            catch (Exception e)
            {
                return new DataResult<ICollection<Product>>()
                {
                    ErrorMessage = e.Message
                };
            }
        }
    }
}