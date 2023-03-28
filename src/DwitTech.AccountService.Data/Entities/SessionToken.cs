using System.ComponentModel.DataAnnotations;

namespace DwitTech.AccountService.Data.Entities
{
    public class SessionToken:BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string RefreshToken{ get; set; }
    }
}
