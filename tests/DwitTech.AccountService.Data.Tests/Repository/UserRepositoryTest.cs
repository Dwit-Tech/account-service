using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
            FirstName = "John",
            LastName = "Doe",
            Email = "info@dwittech.com",
            PhoneNumber = "0802333337",
            AddressLine1 = "Allen",
            AddressLine2 = "Sonubi",
            Country = "Nigeria",
            State = "Lagos",
            City = "Ogba",
            PostalCode = "21356",
            ZipCode = "6564536",
            Status = Enum.UserStatus.Inactive,
        };

           [Fact]
            public void CreateNewUser_Returns_BooleanResult()
            {

                var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

                var dbContext = new AccountDbContext(options);
                var role = new Role { Id = 1, Name = "Admin", Description = "Administrative role" };
                var userModel = new User
                {
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
                    Role = role
                };

                IUserRepository userRepo = new UserRepository(dbContext);

                //Act
                var actual = userRepo.CreateUser(userModel);
                async Task act() => await userRepo.CreateUser(userModel);

                Assert.True(actual.IsCompletedSuccessfully);
                Assert.ThrowsAsync<ArgumentException>(act);
            }

        [Fact]
        public void CreateUser_ThrowsException_When_Supplied_Incomplete_Details()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
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
                Role = role
            };

            IUserRepository userRepo = new UserRepository(dbContext);

            //Act
            async Task act() => await userRepo.CreateUser(userModel);
            Assert.ThrowsAsync<ArgumentException>(act);
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
            var result = await userRepository.GetUserValidationCode("erg3345dh2", CodeType.Activation);

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


        [Fact]
        public async Task SaveUserValidationCode_AddsValidationCodeToDb_WhenValidationCodeIsValid()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var accountDbContext = new AccountDbContext(options))
            {
                var userRepository = new UserRepository(accountDbContext);

                //Act
                await userRepository.SaveUserValidationCode(mockValidationDetails);

                //Assert
                var addedValidationCode = await accountDbContext.ValidationCodes.FindAsync(mockValidationDetails.Id);
                Assert.NotNull(addedValidationCode);
                Assert.Equal(mockValidationDetails, addedValidationCode);
            }
        }


        public void Dispose()
        {
            _accountDbContext.Database.EnsureDeleted();
            _accountDbContext.Dispose();
            
        }
    }
}
