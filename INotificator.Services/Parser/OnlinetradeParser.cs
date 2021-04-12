using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = INotificator.Common.Models.Options;

namespace INotificator.Services.Parser
{
    public class OnlinetradeParser : IOnlinetradeParser
    {
        private readonly ILogger<OnlinetradeParser> _logger;
        private readonly string _url;

        public OnlinetradeParser(IOptions<Options> options,
            ILogger<OnlinetradeParser> logger)
        {
            _logger = logger;
            _url = options?.Value?.Onlinetrade?.Url ??
                   throw new ArgumentNullException(nameof(options.Value.Onlinetrade.Url));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public DataResult<ICollection<Product>> ParseResult(string rawData)
        {
            var results = Regex.Matches(rawData, @"<div class=""indexGoods__item"">(.*?)Купить<\/a><\/div><\/div>");
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
                    var url = Regex.Matches(item.Value, @"<\/div><a href=""(.*?)"" class=""indexGoods__item__name""")
                        .FirstOrDefault();
                    var productName = Regex.Matches(item.Value, @"alt=""(.*?)""><\/a><div ").FirstOrDefault();
                    var price = Regex.Matches(item.Value, @"title=""Клубная цена"">(.*?) &#8381;<\/span>")
                        .FirstOrDefault();

                    if (url != null && productName != null)
                    {
                        result.Add(new Product
                        {
                            Source = _url,
                            Date = DateTime.Now,
                            Name = productName.Groups[1].Value,
                            Price = price.Groups[1].Value?.Replace(" ", string.Empty),
                            Url = $"{_url}{url.Groups[1].Value}"
                        });
                        _logger.LogDebug($"Find item: {productName.Groups[1].Value}");
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