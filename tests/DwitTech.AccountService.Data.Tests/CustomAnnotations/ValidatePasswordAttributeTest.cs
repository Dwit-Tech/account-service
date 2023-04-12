using DwitTech.AccountService.Core.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Tests.CustomAnnotations
{
    public class ValidatePasswordAttributeTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("aB3#")]
        [InlineData("12345678")]
        [InlineData("Abcdefgh")]
        [InlineData("aBcdefg")]
        [InlineData("abcdefgh#")]
        [InlineData("aBcdefgh")]
        public void IsValid_InvalidPassword_ReturnsFalse(string password)
        {
            // Arrange
            var attribute = new ValidatePasswordAttribute();

            // Act
            var result = attribute.IsValid(password);

            // Assert
            Assert.False(result);
        }


        [Theory]
        [InlineData("Abcdefgh#1")]
        [InlineData("1Abcdefgh#")]
        [InlineData("#Abcdefgh1")]
        [InlineData("#1Abcdefgh")]
        [InlineData("aBcdefgh#1")]
        [InlineData("1aBcdefgh#")]
        [InlineData("#aBcdefgh1")]
        [InlineData("#1aBcdefgh")]
        public void IsValid_ValidPassword_ReturnsTrue(string password)
        {
            // Arrange
            var attribute = new ValidatePasswordAttribute();

            // Act
            var result = attribute.IsValid(password);

            // Assert
            Assert.True(result);
        }


        [Theory]
        [InlineData("")]
        [InlineData("aB3#")]
        [InlineData("12345678")]
        [InlineData("Abcdefgh")]
        [InlineData("aBcdefg")]
        [InlineData("abcdefgh#")]
        [InlineData("aBcdefgh")]
        public void FormatErrorMessage_InvalidPassword_ReturnsExpectedMessage(string password)
        {
            // Arrange
            var attribute = new ValidatePasswordAttribute();

            // Act
            var result = attribute.FormatErrorMessage("Password");

            // Assert
            Assert.Equal("The Password field must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, one number, and one special character.", result);
        }
    }
}
