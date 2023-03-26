using System.Security.Cryptography;
using System.Text;

namespace DwitTech.AccountService.Core.Utilities
{
    public static class StringUtil 
    {
        public static string GenerateUniqueCode(int numberOfCharacters=20, bool useNumbers = true, bool useAlphabets = true, bool useSymbols = false)
        {
            var newCharacterOptions = new StringBuilder();

            if (useNumbers)
            {
                newCharacterOptions.Append("1234567890");
            }

            if (useAlphabets)
            {
                newCharacterOptions.Append("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }

            if (useSymbols)
            {
                newCharacterOptions.Append("!#$%&'()*+,-./:;<=>?@[\\]^_`{|}~");
            }

            char[] chars = newCharacterOptions.ToString().ToCharArray();

            byte[] data = new byte[4 * numberOfCharacters];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            var result = new StringBuilder(numberOfCharacters);

            for (int i = 0; i < numberOfCharacters; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();

        }


        public static string HashString(string inputString)
        {
            if (inputString == null)
            {
                return null;
            }

            if (inputString.Equals(""))
            {
                return null;
            }

            if (string.IsNullOrEmpty(inputString)) return null;

            var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));

            return Convert.ToHexString(data).ToLower();
        }
    }
}