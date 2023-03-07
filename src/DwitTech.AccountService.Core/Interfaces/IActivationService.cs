using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
        Task<bool> SendActivationEmail(string userId, string fromEmail, string toEmail, string templateName, string RecipientName,
             string subject = "Account Activation", string cc = "", string bcc = "");
        
    }
}
