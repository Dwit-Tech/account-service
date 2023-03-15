using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(string userId, string FromEmail, string ToEmail, string Body, string Subject, string Cc, string Bcc);
    }
}
