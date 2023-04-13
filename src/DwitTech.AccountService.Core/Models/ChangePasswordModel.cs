using Newtonsoft.Json;

namespace DwitTech.AccountService.Core.Models
{
    public class ChangePasswordModel
    {
        [JsonProperty("currentPassword")]
        public string CurrentPassword { get; set; }
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}
