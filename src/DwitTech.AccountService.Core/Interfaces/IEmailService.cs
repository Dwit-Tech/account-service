using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IEmailService
    {
       public Task<bool> SendEmail(string userId, string FromEmail, string ToEmail, string Body, string Subject, string CC, string BCC);
    }
}
