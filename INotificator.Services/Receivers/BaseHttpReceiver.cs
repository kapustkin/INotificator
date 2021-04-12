using System.Net.Http;
using System.Threading.Tasks;
using INotificator.Common.Interfaces;
using INotificator.Common.Models;
using Microsoft.Extensions.Logging;

namespace INotificator.Services.Receivers
{
    public abstract class BaseHttpReceiver : IReceiver
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;


        protected BaseHttpReceiver( IHttpClientFactory clientFactory,
            ILogger logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        protected virtual Task<string> ReadContentToString(HttpContent content)
        {
            return content.ReadAsStringAsync();
        }

        protected abstract HttpRequestMessage HttpRequest(string path);

        public async Task<DataResult<string>> GetData(string path)
        {
            _logger.LogTrace($"Send request to {path}");
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(HttpRequest(path));
            if (response.IsSuccessStatusCode)
            {
                var result = await ReadContentToString(response.Content);
                _logger.LogTrace($"Response: {result}");

                return new DataResult<string>()
                {
                    Data = result
                };
            } 
            _logger.LogError($"Request failed. Status code: {response.StatusCode}");
            return new DataResult<string>()
            {
                ErrorMessage = $"Request failed. Status code: {response.StatusCode}"
            };
        }
    }
}