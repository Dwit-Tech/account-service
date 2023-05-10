using System.Text.Json.Serialization;

namespace DwitTech.AccountService.Core.Models
{
    public class UserEmailRequestModel
    {
        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }
    }
}
