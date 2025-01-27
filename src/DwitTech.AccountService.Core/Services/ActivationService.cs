using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration; //Config instance for GetBaseUrl method
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IHttpClientFactory _httpClientFactory;

        public ActivationService(IConfiguration configuration,
            IUserRepository userRepository,
            IEmailService emailService,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _httpClientFactory = httpClientFactory;
        }

        private static string GetActivationCode()
        {
            var activationCode = StringUtil.GenerateUniqueCode();
            return activationCode;
        }

        private async Task<string> GetActivationUrl(int userId)
        {
            string baseUrl = _configuration["BASE_URL"];
            string activationCode = GetActivationCode();
            try
            {
                var validationCode = new ValidationCode
                {
                    Code = activationCode,
                    CodeType = CodeType.Activation,
                    UserId = userId,
                    NotificationChannel = NotificationChannel.Email,
                    ModifiedOnUtc = DateTime.UtcNow
                };
                await _userRepository.SaveUserValidationCode(validationCode);
                string activationUrl = baseUrl + "/Account/Activation/" + activationCode;
                return activationUrl;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{ex.Message}");
            }
        }

        public async Task<string> GetTemplate(string templateName)
        {
            string trimmedTemplateName = templateName.Trim();
            var location = new FileInfo(Assembly.GetEntryAssembly().Location);

            string filePath = Path.Combine(location.DirectoryName, "Templates" , trimmedTemplateName);

            var str = new StreamReader(filePath);
            var templateText = await str.ReadToEndAsync();
            str.Close();
            return templateText.ToString();
        }

        public async Task<bool> SendActivationEmail(int userId, string recipientName, Email email, string templateName = "ActivationEmailTemplate.html")
        {
            const string subject = "Account Activation";
            email.Subject = subject;
            var activationUrl = await GetActivationUrl(userId);
            string templateText =await GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", recipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            email.Body = templateText;
            var response = await _emailService.SendMailAsync(email);
            return response;
        }
        public async Task<bool> SendWelcomeEmail(User user)
        {
            string templateText = await GetTemplate("WelcomeEmail.html");
            templateText = templateText.Replace("{{Firstname}}", user.FirstName);
            templateText = templateText.Replace("{{Lastname}}", user.LastName);
            string body = templateText;
            string subject = "Welcome";
            string fromEmail = _configuration["FROM_EMAIL"];
            var email = new Email { FromEmail = fromEmail, ToEmail = user.Email, Subject = subject, Body = body };
            var response = await _emailService.SendMailAsync(email);
            return response;
        }
        private static bool IsUserActivated(User user)
        {
            return user.Status == UserStatus.Active;
        }

        public async Task<bool> ActivateUser(string activationCode)
        {
            ValidationCode activationResult = await _userRepository.GetUserValidationCode(activationCode, CodeType.Activation);
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

            var response = await SendWelcomeEmail(user);
            return response;
        }
    }
}