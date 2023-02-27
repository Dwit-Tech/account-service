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
        [InlineData("userId", "FromEmail", "ToEmail", "Body", "Subject", "CC", "BCC")]
        public async Task Send_Activation_Email(string userId, string FromEmail, string ToEmail, string Body, string Subject, string CC, string BCC)
        {
            //Arrange
            IEmailService EmailService = new EmailService();

            //Act
            var actual = await EmailService.SendEmail(userId, FromEmail, ToEmail, Body, Subject, CC, BCC);

            //Assert
            Assert.True(actual);
        }
    }
}
