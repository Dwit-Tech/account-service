using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Core.Utilities;

namespace DwitTech.AccountService.Core.Tests.Utilities
{
    public class StringUtilTests
    {
        [Fact]
        public void GenerateUniqueCode_GeneratesUniqueCodes()
        {
            // Arrange
            int numberOfCodes = 100;
            var codes = new HashSet<string>();

            // Act
            for (int i = 0; i < numberOfCodes; i++)
            {
                var randomCode = StringUtil.GenerateUniqueCode();
                codes.Add(randomCode);

                //Assert
                Assert.False(string.IsNullOrWhiteSpace(randomCode));                
            }
            Assert.Equal(numberOfCodes, codes.Count);
        }


        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(30)]
        [InlineData(50)]
        public void GenerateUniqueCode_GeneratesCodeWithCorrectLength(int numberOfCharacters)
        {
            // Act
            string code = StringUtil.GenerateUniqueCode(numberOfCharacters);

            // Assert
            Assert.Equal(numberOfCharacters, code.Length);
        }


        [Theory]
        [InlineData(20, false, true, true, 10)]
        [InlineData(20, false, true, true, 5)]
        [InlineData(30, false, false, true, 10)]
        [InlineData(35, false, true, false, 10)]
        public void GenerateUniqueCode_GeneratesCodeWithoutNumbersWhenUseNumbersisFalse(int noOfCharacters, bool useNumbers, bool useAlphabets, bool useSymbols, int expected)
        {
            // Arrange
            var generatedCodes = new HashSet<string>();

            // Act
            for (int i = 0; i < expected; i++)
            {
                string code = StringUtil.GenerateUniqueCode(noOfCharacters, useNumbers, useAlphabets, useSymbols);
                generatedCodes.Add(code);
                Assert.False(string.IsNullOrWhiteSpace(code));
                Assert.DoesNotMatch(@"\d", code);// regex for any digit character
            }

            // Assert
            Assert.Equal(expected, generatedCodes.Count);
        }


        [Theory]
        [InlineData(20, true, false, true, 10)]
        [InlineData(20, true, false, true, 5)]
        [InlineData(30, false, false, true, 10)]
        [InlineData(35, true, false, false, 10)]
        public void GenerateUniqueCode_GeneratesCodeWithoutAlphabetsWhenUseAlphabetsIsFalse(int noOfCharacters, bool useNumbers, bool useAlphabets, bool useSymbols, int expected)
        {
            // Arrange
            var generatedCodes = new HashSet<string>();

            // Act
            for (int i = 0; i < expected; i++)
            {
                string code = StringUtil.GenerateUniqueCode(noOfCharacters, useNumbers, useAlphabets, useSymbols);
                generatedCodes.Add(code);
                Assert.False(string.IsNullOrWhiteSpace(code));
                Assert.DoesNotMatch(@"[a-zA-Z]+", code);// regex for any letter character
            }

            // Assert
            Assert.Equal(expected, generatedCodes.Count);
        }


        [Theory]
        [InlineData(20, true, true, false, 10)]
        [InlineData(20, true, true, false, 5)]
        [InlineData(30, false, true, false, 10)]
        [InlineData(35, true, false, false, 10)]
        public void GenerateUniqueCode_GeneratesCodeWithoutSymbolssWhenUseSymbolsIsFalse(int noOfCharacters, bool useNumbers, bool useAlphabets, bool useSymbols, int expected)
        {
            // Arrange
            var generatedCodes = new HashSet<string>();

            // Act
            for (int i = 0; i < expected; i++)
            {
                string code = StringUtil.GenerateUniqueCode(noOfCharacters, useNumbers, useAlphabets, useSymbols);
                generatedCodes.Add(code);
                Assert.False(string.IsNullOrWhiteSpace(code));
                Assert.DoesNotMatch(@"[!#$%&'()*+,-./:;<=>?@[\\\]^_`{|}~]+", code);// regex for any symbol character
            }

            // Assert
            Assert.Equal(expected, generatedCodes.Count);
        }


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
            var actual = StringUtil.HashString(inputString);

            //Assert
            Assert.Equal(hashString, actual);
        }
    }
}
