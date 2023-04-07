using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Data.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public EmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<bool> SendMailAsync(Email email) 
        {
            var serializedEmail = JsonSerializer.Serialize(email);
            var content = new StringContent(serializedEmail, Encoding.UTF8, "application/json");

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                if (httpClient == null)
                {
                    throw new NullReferenceException("httpClient has no value");
                }
                httpClient.BaseAddress = new Uri(_configuration["NOTIFICATION_SERVICE_BASE_URL"]);
                var response = await httpClient.PostAsync(_configuration["NOTIFICATION_SERVICE_SENDMAIL_END_POINT"], content);
                return (response != null && response.IsSuccessStatusCode);
            }
        }

        
    }
}
