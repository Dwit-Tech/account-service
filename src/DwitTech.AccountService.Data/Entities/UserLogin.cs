using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class UserLogin : BaseEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
