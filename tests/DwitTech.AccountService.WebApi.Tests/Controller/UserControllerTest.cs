using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Enums;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Data.Repository;
using DwitTech.AccountService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.WebApi.Tests.Controller
{
    public class UserControllerTest
    {
        [Fact]
        public async Task CreateUser_Should_Return_True_If_TaskCompletes_Successfully()
        {

             var _userRepository = new Mock<IUserRepository>();
             var _roleRepository = new Mock<IRoleRepository>();
             var _logger = new Mock<ILogger<UserService>>();
             var _activationService = new Mock<IActivationService>();
             var _emailService = new Mock<IEmailService>();

             var userService = new Mock<UserService>(_userRepository.Object, _roleRepository.Object, _logger.Object, _activationService.Object, _emailService.Object);

             var userDto = new UserDto
             {
                    FirstName = "james",
                    LastName = "kim",
                    Email = "test@gmail.com",
                    AddressLine1 = "add1",
                    AddressLine2 = "add2",
                    City = "Bida",
                    State = "Niger",
                    ZipCode = "910001",
                    Roles = Role.User,
                    Country = "Nigeria",
                    PostalCode = "90001",
                    PassWord = "securedpassword",
                    PhoneNumber = "1234567890"
             };

             var userController = new UserController(_activationService.Object, userService.Object);
             var result = userController.CreateUser(userDto);
             Assert.True(result.IsCompleted);
        }
    }
}
