using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class TokenModel
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
