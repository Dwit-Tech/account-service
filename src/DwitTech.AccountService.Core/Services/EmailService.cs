using DwitTech.AccountService.Core.EventsHandler;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DwitTech.AccountService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EventPublisher _eventPublisher;

        public EmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory, EventPublisher eventPublisher)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _eventPublisher = eventPublisher;
        }


        public async Task<bool> SendMailAsync(Email email) //TODO
        {
            var eventData = new EmailSentEvent()
            {
                EmailAddress = email.ToEmail
            };

            _eventPublisher.PublishEvent("email-sent", eventData);
            Debug.WriteLine("Event published successfully.");
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
    }
}
