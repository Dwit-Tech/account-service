using DwitTech.AccountService.Core.Interfaces;

namespace DwitTech.AccountService.Core.Services
{
    public class SecurityService : ISecurityService
    {
<<<<<<< HEAD
        
        public string HashString(string inputString)
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

=======
>>>>>>> 3f3714c76094d5c905c8d46d52bbc2c705b884e1
    }

}