using System.Text.Json.Serialization;

namespace DwitTech.AccountService.Core.Dtos
{
    public class LoginRequestDto
    {
        [JsonPropertyName("username")]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
