﻿using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Tests
{
    public  class SecurityServiceTests 
    {
        [Theory]
        [InlineData("          ", "41b394758330c83757856aa482c79977")]
        [InlineData("I am testing my hash", 
            "4ed43d04e7808a252608996917cbab8")]
        [InlineData(null, null)]
        public void HashString_String_return_Hash(string inputString, string hashString)
        {
            //Arrange
            ISecurityService securityService = new SecurityService();

            //Act
            var actual = securityService.HashString(inputString);

            //Assert
            Assert.Equal(hashString, actual);
        }

    }
}
