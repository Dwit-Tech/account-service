using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class UserServiceTest
    {
        private readonly IConfiguration _configuration;
        public UserServiceTest()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> {

                {"GMAIL_INFO:EMAIL", "johnokpo83@gmail.com"},
                {"GMAIL_INFO:HOST", "smtp.gmail.com"},
                {"GMAIL_INFO:PORT", "587"},
                {"GMAIL_INFO:APP_PASSWORD", "fyvdxxvlsvosecwg"},

            }).Build();
        }


        [Fact]
        public void CreateUser_ShouldReturnTrue_WhenSuccessful()
        {
            var iUserRepoMock = new Mock<IUserRepository>();
            var iLoggerMock = new Mock<ILogger<UserService>>();
            var iRoleRepoMock = new Mock<IRoleRepository>();
            var iActivationServiceMock = new Mock<IActivationService>();
            var iEmailServiceMock = new Mock<IEmailService>();
            var iConfigurationMock = new Mock<IConfiguration>();
            IUserService userServiceUnderTest = new UserService(iUserRepoMock.Object,iRoleRepoMock.Object, iLoggerMock.Object, iActivationServiceMock.Object, iEmailServiceMock.Object, iConfigurationMock.Object);
            var userDto = new UserDto { 
                    FirstName = "james", 
                    LastName = "kim",
                    Email = "test@gmail.com",
                    AddressLine1 = "add1",  
                    AddressLine2 = "add2",
                    City = "Bida",
                    State = "Niger",
                    ZipCode ="910001",
                    Roles = Enums.Role.User,
                    Country = "Nigeria",
                    PostalCode = "90001",
                    PassWord = "securedpassword",
                    PhoneNumber = "1234567890"
                   };


            try
            {
                var act = userServiceUnderTest.CreateUser(userDto);
                Assert.True(act.IsCompleted);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
