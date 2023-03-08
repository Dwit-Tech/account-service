using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration; //Config instance for GetBaseUrl method
        private readonly IUserRepository _userRepository;

        public ActivationService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        private string GetActivationCode()
        {
            var activationCode = RandomUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration.GetSection("BaseUrl").Value;
        }

        private string GetActivationUrl()
        {
            string baseUrl = GetBaseUrl();
            string activationCode = GetActivationCode();
            string activationUrl = baseUrl + "/Account/Activation/" + activationCode;
            return activationUrl;
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

        private static bool SendMail(string fromEmail, string toEmail, string subject, string body, string cc = "", string bcc = "") //TODO
        {
            return true;
        }

        public bool SendActivationEmail(string fromEmail, string toEmail, string templateName, string RecipientName, string subject = "Account Activation", string cc = "", string bcc = "")
        {
            var baseUrl = GetBaseUrl();
            var activationUrl = GetActivationUrl();
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            string body = templateText;
            var response = SendMail(fromEmail, toEmail, subject, body, cc, bcc);

            return response;
        }

        public bool SendWelcomeEmail(string fromEmail, string toEmail, string templateName, string subject = "Account Details", string cc = "", string bcc = "")
        {
            var user = new User();
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{Firstname}}", user.FirstName);
            templateText = templateText.Replace("{{Lastname}}", user.LastName);
            templateText = templateText.Replace("{{Email}}", user.Email);
            templateText = templateText.Replace("{{Password}}", user.Password);
            string body = templateText;
            var response = SendMail(fromEmail, toEmail, subject, body, cc, bcc);

            return response;
        }

        public async Task<bool> ActivateUser(string activationCode, string fromEmail, string toEmail, string templateName, string subject = "Account Details", string cc = "", string bcc = "")
        {
            var activationResult = await _userRepository.GetActivationDetail(activationCode);
            if (activationResult == null)
            {
                return false;
            }
        
            var userStatus = await _userRepository.GetUserStatus(activationResult.UserId);
            
            if (!userStatus)
            {
                throw new Exception("User already Verified.");
            } 

            var validationCode = await _userRepository.ValidateActivationCodeExpiry(activationCode);

            if (!validationCode)
            {
                throw new InvalidOperationException("Activation Code has expired");
            }

            await _userRepository.UpdateUserStatus(activationResult);
            
            var response = SendWelcomeEmail(fromEmail, toEmail, templateName, subject,  cc, bcc);
            return response;
            
        }
    }
}