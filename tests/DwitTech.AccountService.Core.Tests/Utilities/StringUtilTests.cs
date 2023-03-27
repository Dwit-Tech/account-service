﻿using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Core.Services;
using DwitTech.AccountService.Core.Utilities;

namespace DwitTech.AccountService.Core.Tests.Utilities
{
    public class StringUtilTests
    {
        [Fact]
        public void GenerateUniqueCode_Check_Returns_Unique_twenty_Character_Alphanum_string()
        {
            //Arrange
            Dictionary<string, string> valuesDict = new();
            int expected = 100;

            //Act
            for (int i = 0; i < expected; i++)
            {
                string codeValue = StringUtil.GenerateUniqueCode();
                if (!valuesDict.ContainsKey(codeValue))
                {
                    valuesDict[codeValue] = i.ToString();
                }
            }

            //Assert
            Assert.Equal(valuesDict.Count, expected);
        }

        [Fact]
        public void GenerateUniqueCode_Check_Result_Length_Always_Equals_Number_Of_Characters_Parameter_Value()
        {
            //Arrange
            List<int> noOfCharactersOptions = new List<int>() { 15, 22, 30, 42, 50, 38, 6, 28, 17, 46 };
            int expected = noOfCharactersOptions.Count();
            int counter = 0;

            //Act
            foreach (int number in noOfCharactersOptions)
            {
                string resultValue = StringUtil.GenerateUniqueCode(number);
                if (number == resultValue.Length)
                {
                    counter += 1;
                }
            }

            //Assert
            Assert.Equal(expected, counter);
        }

        [Theory]
        [InlineData(20, false, true, true, 10)]
        [InlineData(20, false, true, true, 5)]
        [InlineData(30, false, false, true, 10)]
        [InlineData(35, false, true, false, 10)]
        public void GenerateUniqueCode_Check_Result_Has_No_Numbers_When_useNumbers_isFalse(int noOfCharacters, bool useNumbers, bool useAlphabets, bool useSymbols, int expected)
        {
            //Arrange
            List<string> resultsList = new();
            string numbers = "1234567890";
            int counter = 0;

            //Act      
            if (!useNumbers)
            {
                //loop to generate results {expected} number of times, and populate the list with the results.
                for (int i = 0; i < expected; i++)
                {
                    string result = StringUtil.GenerateUniqueCode(noOfCharacters, useNumbers, useAlphabets, useSymbols);
                    resultsList.Add(result);
                }

                //Check values in the list for characters in numbers and increment counter only if a value has no number.               
                foreach (var value in resultsList)
                {
                    bool matchfound = false;
                    foreach (char number in numbers)
                    {
                        if (value.Contains(number))
                        {
                            matchfound = true;
                            break;
                        }
                    }

                    if (!matchfound)
                    {
                        counter += 1;
                    }
                }
            }

            //Assert
            Assert.Equal(expected, counter);
        }


        [Theory]
        [InlineData(20, true, false, true, 10)]
        [InlineData(20, true, false, true, 5)]
        [InlineData(30, false, false, true, 10)]
        [InlineData(35, true, false, false, 10)]
        public void GenerateUniqueCode_Check_Result_Has_No_Alphabets_When_useAlphabets_isFalse(int noOfCharacters, bool useNumbers, bool useAlphabets, bool useSymbols, int expected)
        {
            //Arrange
            List<string> resultsList = new();
            string alphabets = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int counter = 0;

            //Act      
            if (!useAlphabets)
            {
                //loop to generate results {expected} number of times, and populate the list with the results.
                for (int i = 0; i < expected; i++)
                {
                    string result = StringUtil.GenerateUniqueCode(noOfCharacters, useNumbers, useAlphabets, useSymbols);
                    resultsList.Add(result);
                }

                //Check values in the list for characters in alphabets and increment counter only if a value has no alphabet.               
                foreach (var value in resultsList)
                {
                    bool matchfound = false;
                    foreach (char alphabet in alphabets)
                    {
                        if (value.Contains(alphabet))
                        {
                            matchfound = true;
                            break;
                        }
                    }

                    if (!matchfound)
                    {
                        counter += 1;
                    }
                }
            }

            //Assert
            Assert.Equal(expected, counter);
        }

        [Theory]
        [InlineData(20, true, true, false, 10)]
        [InlineData(20, true, true, false, 5)]
        [InlineData(30, false, true, false, 10)]
        [InlineData(35, true, false, false, 10)]
        public void GenerateUniqueCode_Check_Result_Has_No_Symbols_When_useSymbols_isFalse(int noOfCharacters, bool useNumbers, bool useAlphabets, bool useSymbols, int expected)
        {
            //Arrange
            List<string> resultsList = new();
            string symbols = "!#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            int counter = 0;

            //Act      
            if (!useSymbols)
            {
                //loop to generate results {expected} number of times, and populate the list with the results.
                for (int i = 0; i < expected; i++)
                {
                    string result = StringUtil.GenerateUniqueCode(noOfCharacters, useNumbers, useAlphabets, useSymbols);
                    resultsList.Add(result);
                }

                //Check values in the list for characters in symbols and increment counter only if a value has no symbol.               
                foreach (var value in resultsList)
                {
                    bool matchfound = false;
                    foreach (char symbol in symbols)
                    {
                        if (value.Contains(symbol))
                        {
                            matchfound = true;
                            break;
                        }
                    }

                    if (!matchfound)
                    {
                        counter += 1;
                    }
                }
            }

            //Assert
            Assert.Equal(expected, counter);
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


        [Fact]
        public void GenerateRandomString_Returns_Valid_String_Result()
        {
            //Act
            var result = StringUtil.GenerateRandomBase64string();

            //Assert
            Assert.NotEmpty(result);
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }
    }
}
