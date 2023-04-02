using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Tests.Repository
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly AccountDbContext _accountDbContext;

        ValidationCode mockValidationDetails = new()
        [Fact]
        public void CreateNewUser_Returns_BooleanResult()
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
            var dbContext = new AccountDbContext(options);
            var role = new Role { Id = 1, Name = "Admin", Description = "Administrative role" };
            var userModel = new User
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.ValidationCode.AddAsync(mockValidationDetails);
            await accountDbContext.SaveChangesAsync();
                Id = 1,
                FirstName = "John",
                LastName = "Okpo",
                AddressLine1 = "",
                AddressLine2 = "",
                City = "",
                PhoneNumber = "09023145678",
                ZipCode = "92001",
                PostalCode = "Andrew",
                PassWord = "trionsx",
                Email = "example@gmail.com",
                Country = "Brazil",
                State = "South Casmero",
                Roles = role
            };

            var userRepository = new UserRepository(accountDbContext);
            IUserRepository userRepo = new UserRepository(dbContext);

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
            var actual = userRepo.CreateUser(userModel);
            async Task act() => await userRepo.CreateUser(userModel);

            //Act
            var result = await userRepository.GetUserValidationCode("erg3345dh2", 1);
            Assert.True(actual.IsCompletedSuccessfully);
            Assert.ThrowsAsync<ArgumentException>(act);

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task GetUser_ReturnsUserInfo_WhenCalled()
        public void CreateUser_ThrowsException_When_Supplied_Incomplete_Details()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.Users.AddAsync(mockUser);
            await accountDbContext.SaveChangesAsync();
            var dbContext = new AccountDbContext(options);
            var role = new Role { Id = 1, Name = "Admin", Description = "Administrative role" };
            var userModel = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Okpo",
                AddressLine1 = "",
                PhoneNumber = "09023145678",
                ZipCode = "92001",
                PostalCode = "Andrew",
                PassWord = "trionsx",
                Email = "example@gmail.com",
                Country = "Brazil",
                State = "South Casmero",
                Roles = role
            };

            var userRepository = new UserRepository(accountDbContext);
            IUserRepository userRepo = new UserRepository(dbContext);

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
            async Task act() => await userRepo.CreateUser(userModel);
            Assert.ThrowsAsync<ArgumentException>(act);
        }
    }
}

