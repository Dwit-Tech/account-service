﻿using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DwitTech.AccountService.Data.Tests.Repository
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly AccountDbContext _accountDbContext;

        ValidationCode mockActivationDetails = new()
        {
            Id = 01,
            UserId = 1,
            Code = "erg3345dh2",
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
        public async Task GetUserActivationDetail_Returns_ActivationDetail_WhenActivationDetailExists()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.ValidationCode.AddAsync(mockActivationDetails);
            await accountDbContext.SaveChangesAsync();

            var userRepository = new UserRepository(accountDbContext);

            //Act
            var result = await userRepository.GetUserActivationDetail(mockActivationDetails.Code);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(mockActivationDetails, result);
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
            var result = await userRepository.GetUserActivationDetail("erg3345dh2");

            //Assert
            Assert.Null(result);

        }

        [Fact]
        public async Task GetUserStatus_ReturnsBooleanValue_WhenCalled()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            var userRepository = new UserRepository(accountDbContext);

            //Act
            var actual = await userRepository.GetUserStatus(mockActivationDetails.UserId);

            //Assert
            Assert.IsType<bool>(actual);
        }

        [Fact]
        public async Task ValidateUserActivationCodeExpiry_ReturnsBooleanValue_WhenCalled()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.ValidationCode.AddAsync(mockActivationDetails);
            await accountDbContext.SaveChangesAsync();


            var userRepository = new UserRepository(accountDbContext);

            //Act
            var actual = await userRepository.ValidateUserActivationCodeExpiry(mockActivationDetails.Code);

            //Assert
            Assert.IsType<bool>(actual);
        }

        public void Dispose()
        {
            _accountDbContext.Database.EnsureDeleted();
            _accountDbContext.Dispose();
        }
    }
}

