using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class UserLogin : BaseEntity
    {
        [Required]
        [MaxLength(45)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
    }
}
