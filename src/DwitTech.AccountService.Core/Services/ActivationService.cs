using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration; //Config instance for GetBaseUrl method
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public ActivationService(IConfiguration configuration, IEmailService emailService, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        private string GetActivationCode()
        {
            var activationCode = StringUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration.GetSection("BaseUrl").Value;
        }

        private string GetActivationUrl(int userId)
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

        public async Task<bool> SendActivationEmail(int userId, string templateName, string RecipientName, Email email )
        {
            string subject = "Account Activation";
            email.Subject = subject;
            var activationUrl = GetActivationUrl(userId);
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            string body = templateText;
            email.Body = body;
            var response = await _emailService.SendMailAsync(email);

            return response;
        }

        public async Task<bool> SendWelcomeEmail(User user)
        {

            string templateText = GetTemplate("WelcomeEmail.html");
            templateText = templateText.Replace("{{Firstname}}", user.FirstName);
            templateText = templateText.Replace("{{Lastname}}", user.LastName);
            string body = templateText;
            string subject = "Welcome";
            string fromEmail = _configuration["FROM_EMAIL"];
            var emailModel = new Email { FromEmail = fromEmail, ToEmail = user.Email, Body = templateText, Subject = subject, Cc = "", Bcc = "" };
            var response = await  _emailService.SendMailAsync(emailModel);

            return response;
        }

        private static bool IsUserActivated(User user)
        {
            if (user.Status == Data.Enum.UserStatus.Active)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ActivateUser(string activationCode)
        {
            ValidationCode activationResult = await _userRepository.GetUserValidationCode(activationCode, 1); //CodeType.Activation);
            if (activationResult == null)
            {
                return false;
            }
        
            var user = await _userRepository.GetUser(activationResult.UserId);
            
            if (IsUserActivated(user))
            {
                throw new Exception("User already Activated.");
            }

            DateTime expiredTime = activationResult.CreatedOnUtc.AddMinutes(10);

            if (DateTime.UtcNow > expiredTime)
            {
                throw new InvalidOperationException("Activation Code has expired");
            }

            user.Status = Data.Enum.UserStatus.Active;
            await _userRepository.UpdateUser(user);
            
            var response = SendWelcomeEmail(user);
            return response.Result;
            
        }
    }
}