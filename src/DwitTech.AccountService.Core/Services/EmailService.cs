using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using Microsoft.Extensions.Configuration;

namespace DwitTech.AccountService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEventPublisher _eventPublisher;

        public EmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IEventPublisher eventPublisher)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _eventPublisher = eventPublisher;
        }


        private async Task<bool> SendHttpEmailAsync(Email email)
        {
            return await Task.FromResult(true);
            //TODO Bring up notification service and fix this
            /*
            var serializedEmail = JsonSerializer.Serialize(email);
            var content = new StringContent(serializedEmail, Encoding.UTF8, "application/json");

            using var httpClient = _httpClientFactory.CreateClient();
            if (httpClient == null)
            {
                throw new NullReferenceException("httpClient has no value");
            }
            httpClient.BaseAddress = new Uri(_configuration["NOTIFICATION_SERVICE_BASE_URL"]);
            var response = await httpClient.PostAsync(_configuration["NOTIFICATION_SERVICE_SENDMAIL_END_POINT"], content);
            return (response != null && response.IsSuccessStatusCode);
            */
        }

        public async Task<bool> SendMailAsync(Email email, bool useHttp = false)
        {
            const string topicName = "email-sent";

            if (useHttp)
            {
                var status = await SendHttpEmailAsync(email);

                if (!status)
                {
                    return await _eventPublisher.PublishEmailEventAsync(topicName, email);
                }
                return status;
            }
            return await _eventPublisher.PublishEmailEventAsync(topicName, email);
        }
    }
}
