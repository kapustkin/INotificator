using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Parsers;
using INotificator.Common.Interfaces.Receivers;
using Microsoft.Extensions.Logging;

namespace INotificator.Services.Receivers
{
    public class OnlinetradeReceiver : BaseHttpReceiver, IOnlinetradeReceiver
    {
        public OnlinetradeReceiver(IHttpClientFactory clientFactory, ILogger<OnlinetradeReceiver> logger) : base(clientFactory, logger)
        { }

        protected override async Task<string> ReadContentToString(HttpContent content)
        {
            var buffer = await content.ReadAsByteArrayAsync();
            var byteArray = buffer.ToArray();
            var result = Encoding.GetEncoding("windows-1251").GetString(byteArray, 0, byteArray.Length);
            return result;
        }

        protected override HttpRequestMessage HttpRequest(string path)
        {
            if (path == null )
            {
                throw new NotSupportedException("Path must be non null");
            }
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:86.0) Gecko/20100101 Firefox/86.0");
            return request;
        }
    }
}