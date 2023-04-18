using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.CustomAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]

    public class DifferentFromAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyName;

        public DifferentFromAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherPropertyName);

            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Property '{_otherPropertyName}' not found.");
            }

            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (Equals(value, otherPropertyValue))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
