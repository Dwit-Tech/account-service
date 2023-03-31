using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DwitTech.AccountService.Core.Tests.Services
{
    public class ActivationServiceTests
    {

        [Theory]
        [InlineData("testcase@gmail.com", "example@gmail.com", "EmailTemplate.html", "Mike", true)]
        public void SendActivationEmail_Returns_True(string fromMail, string toMail, string templateName,string RecipientName, bool expected)
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseUrl", "https://example.com"}
            };


            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockDbContext = new Mock<AccountDbContext>(options);
            IUserRepository repository = new UserRepository(mockDbContext.Object);
            
            var actService = new ActivationService(configuration, repository);

            //Act
            var actual = actService.SendActivationEmail(fromMail, toMail, templateName, RecipientName);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ActivateUser_ValidatesActivationCodeSendsWelcomeEmailAndReturnsBooleanValue_WhenCalled()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"FROM_EMAIL", "info@dwittech.com"}
            };


            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            ValidationCode mockValidationDetails = new()
            {
                Id = 01,
                UserId = 1,
                Code = "erg3345dh2",
                CodeType = 1
            };

            User mockUser = new()
            {
                Id = 1,
                Firstname = "John",
                Lastname = "Doe",
                Email = "info@dwittech.com",
                PhoneNumber = "0802333337",
                AddressLine1 = "Allen",
                AddressLine2 = "Sonubi",
                Country = "Nigeria",
                State = "Lagos",
                City = "Ogba",
                PostalCode = "21356",
                ZipCode = "6564536",
                Password = "JeSusIsLord",
                Status = Data.Enum.UserStatus.Inactive
            };

            var mockUserRepository = new Mock<IUserRepository>();


            mockUserRepository.Setup(x => x.GetUserValidationCode(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(mockValidationDetails);
            mockUserRepository.Setup(x => x.GetUser(It.IsAny<int>())).ReturnsAsync(mockUser);
            
            var actService = new ActivationService(configuration, mockUserRepository.Object);

            string activationCode = "erg3345dh2";

            //Act
            var actual = await actService.ActivateUser(activationCode);

            //Assert
            mockUserRepository.Verify(x => x.GetUserValidationCode(activationCode, mockValidationDetails.CodeType), Times.Once);
            mockUserRepository.Verify(x => x.GetUser(It.IsAny<int>()), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);
            Assert.IsType<bool>(actual);
        }
    }
}
