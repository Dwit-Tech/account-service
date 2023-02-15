using DwitTech.AccountService.Core.Interfaces;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Services
{
    public class SecurityService : ISecurityService
    {
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


    }

}