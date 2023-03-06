using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Utilities
{
    public static class RandomUtil 

    {
        internal static readonly string characterOptions = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        
        public static string GenerateUniqueCode(int numberOfCharacters=20, bool useNumbers = true, bool useAlphabets = true, bool useSymbols = false)
        {
            string newCharacterOptions = characterOptions;

            if (!useNumbers)
            {
                string numbers = "1234567890";
                newCharacterOptions = newCharacterOptions.Replace(numbers, "");
            }

            if (!useAlphabets)
            {
                string alphabets = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                newCharacterOptions = newCharacterOptions.Replace(alphabets, "");
            }

            if (!useSymbols)
            {
                string symbols = "!#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
                newCharacterOptions = newCharacterOptions.Replace(symbols, "");
            }

            char[] chars = newCharacterOptions.ToCharArray();

            byte[] data = new byte[4 * numberOfCharacters];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(numberOfCharacters);

            for (int i = 0; i < numberOfCharacters; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();

        }
    }
}