using System;
using System.Net.Http;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Interfaces.Receivers;
using INotificator.Common.Models;
using Microsoft.Extensions.Logging;

namespace INotificator.Services.Receivers
{
    public class LogToApiReceiver : BaseHttpReceiver, ILogToApiReceiver
    {
        private readonly IHttpClientFactory _clientFactory;
        
        public LogToApiReceiver(IHttpClientFactory clientFactory, ILogger<DnsReceiver> logger) : base(clientFactory, logger)
        {
            _clientFactory = clientFactory;
        }

        protected override HttpClient GetHttpClient()
        {
            var client = _clientFactory.CreateClient();
            return client;
        }
        
        protected override HttpRequestMessage HttpRequest(string path)
        {
            if (path == null)
            {
                throw new NotSupportedException("Path must be non null");
            }
            
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            return request;
        }
    }
}