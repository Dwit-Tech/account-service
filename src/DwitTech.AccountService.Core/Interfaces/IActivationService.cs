using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
        
        Task<bool> ActivateUser(string activationCode, User user, string fromEmail, string toEmail, string templateName, string subject, string cc, string bcc);
    }
}
