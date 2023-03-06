using DwitTech.AccountService.Core.Interfaces;
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
        [InlineData("I am testing my hash code", "5c00305ca173f8ae536a344df422bfb8")]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void HashString_String_return_Hash(string inputString, string hashString)
        {
            //Arrange
            ISecurityService SecurityService = new SecurityService();

            //Act
            var actual = SecurityService.HashString(inputString);

            //Assert
            Assert.Equal(hashString, actual);
        }

    }
}
