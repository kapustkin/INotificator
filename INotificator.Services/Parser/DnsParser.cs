using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Services.Parser
{
    public class DnsParser : IDnsParser
    {
        private readonly ILogger<DnsParser> _logger;
        private readonly string _url;

        public DnsParser(IOptions<Options> options,
            ILogger<DnsParser> logger)
        {
            _logger = logger;
            _url = options?.Value?.Dns?.Url ?? throw new ArgumentNullException(nameof(options.Value.Dns.Url));
        }

        public DataResult<ICollection<Product>> ParseResult(string rawData)
        {
            var results = Regex.Matches(rawData, @"<\/div><a class(.*?)<\/a>");
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
                    var url = Regex.Matches(item.Value, "href=\\\\\\\"(.*?)\\\\\\\"><span>").FirstOrDefault();
                    var productName = Regex.Matches(item.Value, @"<span>(.*?)<\/span>").FirstOrDefault();

                    if (url != null && productName != null)
                    {
                        result.Add(new Product
                        {
                            Source = _url,
                            Date = DateTime.Now,
                            Name = productName.Groups[1].Value,
                            Url = $"{_url}{url.Groups[1].Value.Replace("amp;dst=rsu", "dst=rsu")}"
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