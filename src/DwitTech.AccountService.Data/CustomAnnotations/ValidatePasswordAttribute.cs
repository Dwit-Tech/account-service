using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.CustomAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]

    public class ValidatePasswordAttribute : ValidationAttribute
    {
      
        public override bool IsValid(object value)
        {
            string password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Password must be at least 8 characters long
            if (password.Length < 8)
            {
                return false;
            }

            // Password must contain at least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // Password must contain at least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // Password must contain at least one number
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            // Password must contain at least one special character
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, one number, and one special character.";
        }
     
    }
}
