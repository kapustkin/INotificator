using System;
using System.Net.Http;
using System.Threading.Tasks;
using INotificator.Common.Interfaces.Receivers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INotificator.Services.Receivers
{
    public class AvitoReceiver:BaseHttpReceiver, IAvitoReceiver
    {
        public AvitoReceiver(IHttpClientFactory clientFactory, ILogger<AvitoReceiver> logger) : base(clientFactory, logger)
        {
        }
        
        protected override HttpRequestMessage HttpRequest(string path)
        {
            if (path == null)
            {
                throw new NotSupportedException("Path must be non null");
            }
            
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:86.0) Gecko/20100101 Firefox/86.0");
            return request;
        }
    }
}