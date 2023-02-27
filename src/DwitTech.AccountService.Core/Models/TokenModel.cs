using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Models
{
    public class TokenModel
    {
        [JsonPropertyName("access_token")]
        public string accessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string tokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public int expiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string refreshToken { get; set; }
    }
}
