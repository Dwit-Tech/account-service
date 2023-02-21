using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IEmailService
    {
        bool SendEmail(string FromEmail, string ToEmail, string Body, string Subject, string CC, string BCC);
    }
}
