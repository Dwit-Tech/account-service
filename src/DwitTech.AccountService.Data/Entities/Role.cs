using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class Role : BaseEntity
    {
        public RoleName Roles { get; set; }
    }
}
