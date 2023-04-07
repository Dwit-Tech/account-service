using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Models
{
    public class Email
    {
        public string FromEmail { get;  set; }
        public string ToEmail { get;  set; }
        public string Subject { get;  set; }
        public string Body { get;  set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
    }
}
