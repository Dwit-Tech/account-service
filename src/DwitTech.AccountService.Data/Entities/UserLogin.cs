using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class UserLogin : BaseEntity
    {

        [Required]
        [MaxLength(45)]
        public string Email { get; set; }

        [Required]
        [Range(8, 16)]
        public string PassWord { get; set; }

        public User User { get; set; }
    }
}
