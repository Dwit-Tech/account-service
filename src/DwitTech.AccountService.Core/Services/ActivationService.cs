﻿using System.Text;
using System.Text.Json;
using DwitTech.AccountService.Core.Enums;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;

namespace DwitTech.AccountService.Core.Services
{
    public class ActivationService : IActivationService
    {
        private readonly IConfiguration _configuration; //Config instance for GetBaseUrl method
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _endpointUrl;

        public ActivationService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        private string GetActivationCode()
        {
            var activationCode = StringUtil.GenerateUniqueCode();
            return activationCode;
        }

        private string GetBaseUrl()
        {
            return _configuration["BASE_URL"];
        }

        private async Task<string> GetActivationUrl(int userId)
        {
            string baseUrl = GetBaseUrl();
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
                var saveResponse = await _userRepository.SaveUserValidationCode(validationCode);
                string activationUrl = baseUrl + "/Account/Activation/" + activationCode;
                return activationUrl;
            }
            catch (Exception ex)
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

        public async Task<bool> SendActivationEmail(int userId, string templateName, string RecipientName, Email email)
        {
            const string subject = "Account Activation";
            email.Subject = subject;
            var activationUrl = await GetActivationUrl(userId);
            string templateText = GetTemplate(templateName);
            templateText = templateText.Replace("{{name}}", RecipientName);
            templateText = templateText.Replace("{{activationUrl}}", activationUrl);
            email.Body = templateText;
            var response = await SendMailAsync(email);
            return response;
        }

        public async Task<bool> SendMailAsync(Email email)
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

        public bool SendWelcomeEmail(User user)
        {

            string templateText = GetTemplate("WelcomeEmail.html");
            templateText = templateText.Replace("{{Firstname}}", user.Firstname);
            templateText = templateText.Replace("{{Lastname}}", user.Lastname);
            string body = templateText;
            string subject = "Welcome";
            string fromEmail = _configuration["FROM_EMAIL"];
            var response = SendMail(fromEmail, user.Email, subject, body);

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
            return response;
            
        }
    }
}