using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
        bool SendActivationEmail(string fromEmail, string toEmail, string templateName, string RecipientName, string subject, string cc, string bcc);
    }
}
