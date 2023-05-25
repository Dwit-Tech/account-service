using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Models
{
    public class PasswordResetModel
    {
        [Required]
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }

        [Required]
        [JsonPropertyName("confirmPassword")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
