using System.Net.Http;
using System.Text;
using System.Text.Json;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _endpointUrl; // TODO

        public ActivationService(IConfiguration configuration, IUserRepository userRepository, IHttpClientFactory httpClientFactory, string endpointUrl)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _endpointUrl = endpointUrl;
        }

        private static string GetActivationCode()
        {
            var activationCode = StringUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration["BaseUrl"];
        }

        private async Task<string> GetActivationUrl(int userId)
        {
            string baseUrl = GetBaseUrl();
            string activationCode = GetActivationCode();
            try {
                var saveResponse = await _userRepository.SaveUserValidationCode(userId, activationCode);
                string activationUrl = baseUrl + "/Account/Activation/" + activationCode;
                return activationUrl;
            }catch(Exception ex)
            {
                throw new ArgumentException($"{ex.Message}");
            }
            
        }

        private string GetTemplate(string templateName)
        {
            string trimmedTemplateName = templateName.Trim();
            string filePath = "Templates/" + trimmedTemplateName;
            StreamReader str = new StreamReader(filePath);
            var templateText = str.ReadToEnd();
            str.Close();
            return templateText.ToString();
        }
 
        public async Task<bool> SendActivationEmail(int userId, string templateName, string RecipientName, EmailDto emailDto)
        {
            const string subject = "Account Activation";

            emailDto.Subject = subject;
            var activationUrl = await GetActivationUrl(userId);
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            emailDto.Body = templateText;
            var response = await SendMailAsync(emailDto);

            return response;

        }

        public async Task<bool> SendMailAsync(EmailDto emailDto)
        {
            var serializedEmail = JsonSerializer.Serialize(emailDto);
            var content = new StringContent(serializedEmail, Encoding.UTF8, "application/json");

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.PostAsync(_endpointUrl, content);
                return response.IsSuccessStatusCode;
            }
        }
    }
}