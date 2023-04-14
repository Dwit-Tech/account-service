using DwitTech.AccountService.Data.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Tests.CustomAnnotations
{
    public class DifferentFromAttributeTest
    {
        public class TestModel
        {
            public string Password { get; set; }
            [DifferentFrom(nameof(Password), ErrorMessage = "New password must be different from old password.")]
            public string NewPassword { get; set; }
        }

        [Fact]
        public void IsValid_ReturnsSuccess_WhenValuesAreDifferent()
        {
            // Arrange
            var testModel = new TestModel { Password = "oldPassword", NewPassword = "newPassword" };
            var validationContext = new ValidationContext(testModel)
            {
                MemberName = nameof(TestModel.NewPassword)
            };
            var differentFromAttribute = new DifferentFromAttribute(nameof(TestModel.Password));

            // Act
            var validationResult = differentFromAttribute.GetValidationResult(testModel.NewPassword, validationContext);

            // Assert
            Assert.Equal(ValidationResult.Success, validationResult);
        }

        [Fact]
        public void IsValid_ReturnsError_WhenValuesAreEqual()
        {
            // Arrange
            var testModel = new TestModel { Password = "password", NewPassword = "password" };
            var validationContext = new ValidationContext(testModel)
            {
                MemberName = nameof(TestModel.NewPassword)
            };
            var differentFromAttribute = new DifferentFromAttribute(nameof(TestModel.Password))
            {
                ErrorMessage = "New password must be different from old password."
            };

            // Act
            var validationResult = differentFromAttribute.GetValidationResult(testModel.NewPassword, validationContext);

            // Assert
            Assert.Equal("New password must be different from old password.", validationResult.ErrorMessage);
        }

        [Fact]
        public void IsValid_ReturnsError_WhenOtherPropertyNotFound()
        {
            // Arrange
            var testModel = new TestModel { Password = "password", NewPassword = "newPassword" };
            var validationContext = new ValidationContext(testModel)
            {
                MemberName = nameof(TestModel.NewPassword)
            };
            var differentFromAttribute = new DifferentFromAttribute("InvalidPropertyName");

            // Act
            var validationResult = differentFromAttribute.GetValidationResult(testModel.NewPassword, validationContext);

            // Assert
            Assert.Equal("Property 'InvalidPropertyName' not found.", validationResult.ErrorMessage);
        }
    }
}
