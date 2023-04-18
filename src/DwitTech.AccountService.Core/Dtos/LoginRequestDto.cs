using DwitTech.AccountService.Core.CustomAnnotations;
using System.Text.Json.Serialization;

namespace DwitTech.AccountService.Core.Dtos
{
    public class LoginRequestDto
    {
        [JsonPropertyName("username")]
        public string Email { get; set; }

        [ValidatePassword]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
