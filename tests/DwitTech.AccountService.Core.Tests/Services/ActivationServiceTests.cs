using Castle.Core.Configuration;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationServiceTests
    {
        [Theory]
       [InlineData("2", "testcase@gmail.com", "example@gmail.com", "EmailTemplate.html", "Mike", true)]

        public async Task SendActivationEmail_Returns_True(string userId, string fromMail, string toMail, string templateName, string RecipientName, bool expected)
        
        {
            var _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {


            }).Build();

            var userRepository = new Mock<IUserRepository>();
            IActivationService activationService = new ActivationService(_configuration, userRepository.Object);
            var result = await activationService.SendActivationEmail(userId, fromMail, toMail, templateName, RecipientName);
            Assert.True(result);




        }
    }
}
