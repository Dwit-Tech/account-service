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
    public class UserRepositoryTest
    {
        [Fact]
        public void CreateNewUser_Returns_BooleanResult()
        {
            
            //Arrange
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
                Country = "Brazil",
                State = "South Casmero",
                Roles = role
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
                Country = "Brazil",
                State = "South Casmero",
                Roles = role
            };

            IUserRepository userRepo = new UserRepository(dbContext);

            //Act
            async Task act() => await userRepo.CreateUser(userModel);
            Assert.ThrowsAsync<ArgumentException>(act);
        }
    }
}
