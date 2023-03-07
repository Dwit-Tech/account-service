using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationEmailTests
    {

        [Theory]
        [InlineData("userId=2", "testcase@gmail.com", "example@gmail.com", "EmailTemplate.html", "sendActivationEmail", "cc", "bcc")]
        public async Task Send_Activation_Email(string userId, string fromEmail, string toEmail, string body, string subject, string cc, string bcc)
        {
            //Arrange
            IEmailService emailService = new EmailService();

            //Act
            var actual = await emailService.SendEmail(userId, fromEmail, toEmail, body, subject, cc, bcc);

            //Assert
            Assert.True(actual);
        }
    }
}
