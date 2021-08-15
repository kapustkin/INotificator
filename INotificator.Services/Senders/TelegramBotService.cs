using System;
using System.Threading.Tasks;
using INotificator.Common.Models;
using INotificator.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace INotificator.Services.Senders
{
    public class TelegramBotService : ISender
    {
        private readonly ILogger _logger;
        private readonly Common.Models.Options _options;

        private static TelegramBotClient _botClient;

        public TelegramBotService(ILogger<TelegramBotService> logger, IOptions<Common.Models.Options> options)
        {
            _logger = logger;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (_botClient != null) return;

            _botClient = new TelegramBotClient(_options.ApiKey);
            _botClient.OnMessage += OnMessage;
            _botClient.StartReceiving();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                _logger.LogDebug($"New message: {e.Message.Text}");
            }
        }

        public Task<bool> Send(Message message)
        {
            return Send(message, false);
        }

        public async Task<bool> Send(Message message, bool disableNotification)
        {
            try
            {
                var result = await _botClient.SendTextMessageAsync(
                    chatId: new Telegram.Bot.Types.ChatId(_options.ChatId),
                    text: $"{message.MessageText} {message.Link}",
                    disableNotification: disableNotification
                );

                _logger.LogInformation($"Send message {message.Source} {message.MessageText}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send message error {ex.Message} ");
                return false;
            }
        }
    }
}