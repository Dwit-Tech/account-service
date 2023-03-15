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
        private readonly IEmailService _emailService;


        public ActivationService(IConfiguration configuration, IUserRepository userRepository, IEmailService emailService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public ActivationService(IConfigurationRoot configuration, IUserRepository @object)
        {
        }

        private static string GetActivationCode()
        {
            var activationCode = StringUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration.GetSection("BaseUrl").Value;
        }

        private async Task<string> GetActivationUrl(string userId)
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

        
        public async Task<bool> SendActivationEmail(string userId, string fromEmail, string toEmail, string templateName, string RecipientName,
            string cc = "", string bcc = "")
        {
            const string Subject = "Account Activation";
           
           var activationUrl = await GetActivationUrl(userId);
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            string body = templateText;
            var response = await _emailService.SendMailAsync(userId, fromEmail, toEmail, Subject, body, cc, bcc);
         

            return response;
        }
    }
}