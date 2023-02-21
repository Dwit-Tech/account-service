using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class SessionToken:BaseEntity
    {
        public int UserId { get; set; }
        public string RefreshToken{ get; set; }
    }
}
