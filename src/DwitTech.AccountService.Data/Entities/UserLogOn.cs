using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class UserLogOn:BaseEntity
    {
        public int UserId { get; set; }
        public string RefreshToken{ get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
