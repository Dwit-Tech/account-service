using DwitTech.AccountService.Core.CustomAnnotations;
using DwitTech.AccountService.Data.CustomAnnotations;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace DwitTech.AccountService.Core.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [JsonPropertyName("currentPassword")]
        public string CurrentPassword { get; set; }

        [Required]
        [JsonPropertyName("newPassword")]
        [ValidatePassword]
        [DifferentFrom(nameof(CurrentPassword), ErrorMessage = "New password must be different from current password.")]        
        public string NewPassword { get; set; }

        [Required]
        [JsonPropertyName("confirmNewPassword")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
