using DwitTech.AccountService.Core.CustomAnnotations;
using DwitTech.AccountService.Data.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DwitTech.AccountService.Core.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePassword]
        [DifferentFrom(nameof(OldPassword), ErrorMessage = "New password must be different from old password.")]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
