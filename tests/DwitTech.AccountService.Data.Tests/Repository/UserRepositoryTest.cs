using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DwitTech.AccountService.Data.Tests.Repository
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly AccountDbContext _accountDbContext;

        ValidationCode mockValidationDetails = new()
        {
            Id = 01,
            UserId = 1,
            Code = "erg3345dh2",
            CodeType = Enum.CodeType.Activation
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
            Status = Enum.UserStatus.Inactive,
        };

        public Mock<IUserRepository> mockUserRepository;

        public UserRepositoryTest()
        {

            var options = new DbContextOptionsBuilder<AccountDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            _accountDbContext = new AccountDbContext(options);
            _accountDbContext.Database.EnsureCreated();

            _accountDbContext.SaveChanges();

            mockUserRepository = new Mock<IUserRepository>();

        }


        [Fact]
        public async Task GetUserValidationCode_Returns_ValidationCodeDetails_WhenActivationCodeExists()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.ValidationCodes.AddAsync(mockValidationDetails);
            await accountDbContext.SaveChangesAsync();

            var userRepository = new UserRepository(accountDbContext);

            //Act
            var result = await userRepository.GetUserValidationCode(mockValidationDetails.Code, mockValidationDetails.CodeType);           

            //Assert
            Assert.NotNull(result);
            Assert.Equal(mockValidationDetails, result);
        }

        [Fact]
        public async Task GetUserActivationDetail_Returns_Null_WhenActivationDetailDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            var userRepository = new UserRepository(accountDbContext);

            //Act
            var result = await userRepository.GetUserValidationCode("erg3345dh2", 1);

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task GetUser_ReturnsUserInfo_WhenCalled()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.Users.AddAsync(mockUser);
            await accountDbContext.SaveChangesAsync();

            var userRepository = new UserRepository(accountDbContext);

            //Act
            var actual = await userRepository.GetUser(mockUser.Id);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(mockUser, actual);
        }

        public void Dispose()
        {
            _accountDbContext.Database.EnsureDeleted();
            _accountDbContext.Dispose();
        }
    }
}
