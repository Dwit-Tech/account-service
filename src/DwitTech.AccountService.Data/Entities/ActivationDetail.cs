using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Entities
{
    public class ActivationDetail : BaseEntity
    {
        public string ActivationCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
